"""
Full code for running a game of tic-tac-toe on a board of any size with a specified number in a row for the win. This is
similar to tic_tac_toe.py but all relevent moves are paramiterized by board_size arg that sets how big the board is and
winning_length which determines how many in a row are needed to win. Defaults are 5 and 4. This allows you to play games
in a more complex environment than standard tic-tac-toe.

Two players take turns making moves on squares of the board, the first to get winning_length in a row, including
diagonals, wins. If there are no valid moves left to make the game ends a draw.

The main method to use here is play_game which simulates a game to the end using the function args it takes to determine
where each player plays.
The board is represented by a board_size x board_size tuple of ints. A 0 means no player has played in a space, 1 means
player one has played there, -1 means the seconds player has played there. The apply_move method can be used to return a
copy of a given state with a given move applied. This can be useful for doing min-max or monte carlo sampling.
"""
import sys
import itertools
import random
import time
import numpy as np
import tensorflow as tf
import pickle
import operator
from functools import reduce
import math

from base_game_spec import BaseGameSpec
from network_helpers import create_network
#from network_helpers import get_stochastic_network_move
#import network_helpers


def _new_board(board_size):
    """Return a emprty tic-tac-toe board we can use for simulating a game.

    Args:
        board_size (int): The size of one side of the board, a board_size * board_size board is created

    Returns:
        board_size x board_size tuple of ints
    """
    return tuple(tuple(0 for _ in range(board_size)) for _ in range(board_size))


def apply_move(board_state, move, side):
    """Returns a copy of the given board_state with the desired move applied.

    Args:
        board_state (2d tuple of int): The given board_state we want to apply the move to.
        move (int, int): The position we want to make the move in.
        side (int): The side we are making this move for, 1 for the first player, -1 for the second player.

    Returns:
        (2d tuple of int): A copy of the board_state with the given move applied for the given side.
    """
    #print(move)

    if len(move) == 4:
        move_x, move_y, quarter, direction = move
        print("x: %s", move_x)
        print("y: %s", move_y)
    else: 
        move_x, move_y = move
        print("x: %s", move_x)
        print("y: %s", move_y)
        quarter, direction = t_handler(board_state)


    def get_tuples():
        for x in range(len(board_state)):
            if move_x == x:
                temp = list(board_state[x])
                temp[move_y] = side
                yield tuple(temp)
            else:
                yield board_state[x]

    board_state = tuple(get_tuples())
    board_state = quarter_turn(quarter, direction, board_state)
    return board_state

def user_input_apply_move(board_state, move, side):
    """Returns a copy of the given board_state with the desired move applied.

    Args:
        board_state (2d tuple of int): The given board_state we want to apply the move to.
        move (int, int): The position we want to make the move in.
        side (int): The side we are making this move for, 1 for the first player, -1 for the second player.

    Returns:
        (2d tuple of int): A copy of the board_state with the given move applied for the given side.
    """
    #print(move)

    move_x, move_y = move

    def get_tuples():
        for x in range(len(board_state)):
            if move_x == x:
                temp = list(board_state[x])
                temp[move_y] = side
                yield tuple(temp)
            else:
                yield board_state[x]

    board_state = tuple(get_tuples())
    return board_state

def available_moves(board_state):
    """Get all legal moves for the current board_state. For Tic-tac-toe that is all positions that do not currently have
    pieces played.

    Args:
        board_state: The board_state we want to check for valid moves.

    Returns:
        Generator of (int, int): All the valid moves that can be played in this position.
    """
    #_available_moves = []

    for x, y in itertools.product(range(len(board_state)), range(len(board_state[0]))):
        if board_state[x][y] == 0:
            yield(x,y)


def _has_winning_line(line, winning_length):
    count = 0
    last_side = 0
    for x in line:
        if x == last_side:
            count += 1
            if count == winning_length:
                return last_side
        else:
            count = 1
            last_side = x
    return 0


def has_winner(board_state, winning_length):
    """Determine if a player has won on the given board_state.

    Args:
        board_state (2d tuple of int): The current board_state we want to evaluate.
        winning_length (int): The number of moves in a row needed for a win.

    Returns:
        int: 1 if player one has won, -1 if player 2 has won, otherwise 0.
    """
    board_width = len(board_state)
    board_height = len(board_state[0])

    # check rows
    for x in range(board_width):
        winner = _has_winning_line(board_state[x], winning_length)
        if winner != 0:
            return winner
    # check columns
    for y in range(board_height):
        winner = _has_winning_line((i[y] for i in board_state), winning_length)
        if winner != 0:
            return winner

    # check diagonals
    diagonals_start = -(board_width - winning_length)
    diagonals_end = (board_width - winning_length)
    for d in range(diagonals_start, diagonals_end + 1):
        winner = _has_winning_line(
            (board_state[i][i + d] for i in range(max(-d, 0), min(board_width, board_height - d))),
            winning_length)
        if winner != 0:
            return winner
    for d in range(diagonals_start, diagonals_end + 1):
        winner = _has_winning_line(
            (board_state[i][board_height - i - d - 1] for i in range(max(-d, 0), min(board_width, board_height - d))),
            winning_length)
        if winner != 0:
            return winner

    return 0  # no one has won, return 0 for a draw


def _evaluate_line(line, winning_length):
    count = 0
    last_side = 0
    score = 0
    neutrals = 0

    for x in line:
        if x == last_side:
            count += 1
            if count == winning_length and neutrals == 0:
                return 100000 * x  # a side has already won here
        elif x == 0:  # we could score here
            neutrals += 1
        elif x == -last_side:
            if neutrals + count >= winning_length:
                score += (count - 1) * last_side
            count = 1
            last_side = x
            neutrals = 0
        else:
            last_side = x
            count = 1

    if neutrals + count >= winning_length:
        score += (count - 1) * last_side

    return score

def t_handler(board_state):
    max_eval = 0
    max_turn = 0,0
    for q in range(4):
        direction_switch = 1
        for d in range(2):
            if d == 0: 
                direction_switch = -1
            t_board_state = quarter_turn(q, direction_switch, board_state)
            t_board_eval = evaluate(t_board_state, 5)
            if t_board_eval > max_eval:
                max_eval = t_board_eval
                max_turn = q, direction_switch

    print("quarter: ", max_turn[0])

    print("direction: ", max_turn[1])
    return max_turn


def evaluate(board_state, winning_length):
    """An evaluation function for this game, gives an estimate of how good the board position is for the plus player.
    There is no specific range for the values returned, they just need to be relative to each other.

    Args:
        winning_length (int): The length needed to win a game
        board_state (tuple): State of the board

    Returns:
        number
    """
    board_width = len(board_state)
    board_height = len(board_state[0])

    score = 0

    # check rows
    for x in range(board_width):
        score += _evaluate_line(board_state[x], winning_length)
    # check columns
    for y in range(board_height):
        score += _evaluate_line((i[y] for i in board_state), winning_length)

    # check diagonals
    diagonals_start = -(board_width - winning_length)
    diagonals_end = (board_width - winning_length)
    for d in range(diagonals_start, diagonals_end + 1):
        score += _evaluate_line(
            (board_state[i][i + d] for i in range(max(-d, 0), min(board_width, board_height - d))),
            winning_length)
    for d in range(diagonals_start, diagonals_end + 1):
        score += _evaluate_line(
            (board_state[i][board_height - i - d - 1] for i in range(max(-d, 0), min(board_width, board_height - d))),
            winning_length)

    return score


def play_game(plus_player_func, minus_player_func, session, board_size=6, winning_length=5, log=False):
    """Run a single game of tic-tac-toe until the end, using the provided function args to determine the moves for each
    player.

    Args:
        plus_player_func ((board_state(board_size by board_size tuple of int), side(int)) -> move((int, int))):
            Function that takes the current board_state and side this player is playing, and returns the move the player
            wants to play.
        minus_player_func ((board_state(board_size by board_size tuple of int), side(int)) -> move((int, int))):
            Function that takes the current board_state and side this player is playing, and returns the move the player
            wants to play.
        board_size (int): The size of a single side of the board. Game is played on a board_size*board_size sized board
        winning_length (int): The number of pieces in a row needed to win a game.
        log (bool): If True progress is logged to console, defaults to False

    Returns:
        int: 1 if the plus_player_func won, -1 if the minus_player_func won and 0 for a draw
    """
    board_state = _new_board(board_size)
    player_turn = 1
    #with tf.Session() as session:
    while True:
      
        if player_turn == 1:
            print("user turn")
        else:
            print("cpu turn")

        print("board state:")
        print(np.array(board_state))

        _available_moves = list(available_moves(board_state))

        if len(_available_moves) == 0:
            # draw
            if log:
                print("no moves left, game ended a draw")
            return 0.
        if player_turn > 0:
            move = plus_player_func(board_state, 1)
        else:
            move = one_hot_to_move((minus_player_func(session, input_layer, output_layer, board_state, -1)))

        placement_move = move[0:2]
        if placement_move not in _available_moves:
            # if a player makes an invalid move the other player wins
            if log:
                print("illegal move ", placement_move)
            return -player_turn

        board_state = apply_move(board_state, move, player_turn)
        #rotation
        #if player_turn > 0:

        winner = has_winner(board_state, winning_length)
        if winner != 0:
            if log:
                print("we have a winner, side: %s" % player_turn)
            return winner
        player_turn = -player_turn

def one_hot_to_move(one_hot_vec):
    index = 0
    while one_hot_vec[index] != 1:
        index += 1

    x_value = index//6
    y_value = index%6
    #quarter = one_hot_vec[36]
    #direction = one_hot_vec[37]
    return x_value, y_value#, quarter, direction

def random_player(board_state, _):
    """A player func that can be used in the play_game method. Given a board state it chooses a move randomly from the
    valid moves in the current state.

    Args:
        board_state (2d tuple of int): The current state of the board
        _: the side this player is playing, not used in this function because we are simply choosing the moves randomly

    Returns:
        (int, int): the move we want to play on the current board
    """
    moves = list(available_moves(board_state))
    #print(quarter)
    #print(direction)

    move = random.choice(moves)
    m1 = move[0]
    m2 = move[1]

    return m1, m2#, quarter, direction


class TicTacToeXGameSpec(BaseGameSpec):
    def __init__(self, board_size, winning_length):
        """

        Args:
            board_size (int): The length of one side of the board, so the bard will have board_size*board_size total
                squares
            winning_length (int): The length in a row needed to win the game. Should be less than or equal to board_size
        """
        if not isinstance(board_size, int):
            raise TypeError("board_size must be an int")
        if not isinstance(winning_length, int):
            raise TypeError("winning_length must be an int")
        if winning_length > board_size:
            raise ValueError("winning_length must be less than or equal to board_size")
        self._winning_length = winning_length
        self._board_size = board_size
        self.available_moves = available_moves
        self.apply_move = apply_move

    def new_board(self):
        return _new_board(self._board_size)

    def has_winner(self, board_state):
        return has_winner(board_state, self._winning_length)

    def board_dimensions(self):
        return self._board_size, self._board_size

    def evaluate(self, board_state):
        return evaluate(board_state, self._winning_length)

def user_input(board_state, _):
    print("Enter y")
    move_y = int(input())
    print("Enter x")
    move_x = int(input())
    print("Enter the rotation quarter")
    quarter = int(input())
    print("Enter the rotation direction")
    direction = int(input())

    return move_y, move_x, quarter, direction

def quarter_turn(quarter, direction, board_state):
    #-1 = counterclockwise (left), 1 = clockwise (right)
    map(list, board_state)
    board_state = np.array(board_state)
    rotIndex = [0,1,2,6,7,8,12,13,14]
    leftRotIndex = [12,6,0,13,7,1,14,8,2]
    rightRotIndex = [2,8,14,1,7,13,0,6,12]
    
    if quarter == 1:
        quarter = 2
    elif quarter == 2:
        quarter = 1

    baseForIndex = 0
    if quarter > 1:
        baseForIndex += 18
    if quarter == 1 or quarter == 3:
        baseForIndex += 3


    tempVals = np.zeros(9)

    for i in range(9):
        fromSpot = rotIndex[i] + baseForIndex
        fromX = fromSpot % 6
        fromY = fromSpot //6
        tempVals[i] = board_state[fromX][fromY]

    if direction > 0:
        for i in range(9):
            x = (leftRotIndex[i]+baseForIndex) % 6
            y = (leftRotIndex[i]+baseForIndex) // 6
            board_state[x][y] = tempVals[i]
    else:
        for i in range(9):
            x = (rightRotIndex[i]+baseForIndex) % 6
            y = (rightRotIndex[i]+baseForIndex) // 6
            board_state[x][y] = tempVals[i]
    tuple(map(tuple, board_state))
    return board_state

def get_stochastic_network_move_x(session, input_layer, output_layer, board_state, side,
                                valid_only=True, game_spec=TicTacToeXGameSpec(6,5)):
    """Choose a move for the given board_state using a stocastic policy. A move is selected using the values from the
     output_layer as a categorical probability distribution to select a single move

    Args:
        session (tf.Session): Session used to run this network
        input_layer (tf.Placeholder): Placeholder to the network used to feed in the board_state
        output_layer (tf.Tensor): Tensor that will output the probabilities of the moves, we expect this to be of
            dimesensions (None, board_squares) and the sum of values across the board_squares to be 1.
        board_state: The board_state we want to get the move for.
        side: The side that is making the move.

    Returns:
        (np.array) It's shape is (board_squares), and it is a 1 hot encoding for the move the network has chosen.
    """
    np_board_state = np.array(board_state)
    #if side == -1:
    np_board_state = -np_board_state

    np_board_state = np_board_state.reshape(1, *input_layer.get_shape().as_list()[1:])

    probability_of_actions = session.run(output_layer,
                                         feed_dict={input_layer: np_board_state})[0]

    if valid_only:
        available_moves = list(game_spec.available_moves(board_state))
        if len(available_moves) == 1:
            move = np.zeros(game_spec.board_squares())
            np.put(move, game_spec.tuple_move_to_flat(available_moves[0]), 1)
            quarter = random.randint(-1,3)
            direction = np.exp(-1, random.randint(0,2))
            move = np.append(move, quarter)
            move = np.append(move, direction)
            return move
        available_moves_flat = [game_spec.tuple_move_to_flat(x) for x in available_moves]
        for i in range(game_spec.board_squares()):
            if i not in available_moves_flat:
                probability_of_actions[i] = 0.

        prob_mag = sum(probability_of_actions)
        if prob_mag != 0.:
            probability_of_actions /= sum(probability_of_actions)

    try:
        move = np.random.multinomial(1, probability_of_actions)
    except ValueError:
        # sometimes because of rounding errors we end up with probability_of_actions summing to greater than 1.
        # so need to reduce slightly to be a valid value
        move = np.random.multinomial(1, probability_of_actions / (1. + 1e-6))

    return move

def flat_move_to_tuple(self, move_index):
        """If board is 2d then we return a tuple for where we moved to.
        e.g if the board is a 3x3 size and our move_index was 6 then
        this method will return (2, 0)

        Args:
            move_index (int): The index of the square we moved to

        Returns:
            tuple or int: For where we moved in board coordinates
        """
        if len(self.board_dimensions()) == 1:
            return move_index

        board_x = self.board_dimensions()[0]
        return int(move_index / board_x), move_index % board_x

def load_network(session, tf_variables, file_path):
    """Load the given set of variables from the given file using the given session

    Args:
        session (tf.Session): session within which the variables has been initialised
        tf_variables (list of tf.Variable): list of variables which will set up with the values saved to the file. List
            order matters, in must be the exact same order as was used to save and all of the same shape.
        file_path (str): path of the file we want to load from.
    """
    with open("testnetworkP2.p", mode='rb') as f:
        variable_values = pickle.load(f)

    try:
        if len(variable_values) != len(tf_variables):
            raise ValueError("Network in file had different structure, variables in file: %s variables in memeory: %s"
                             % (len(variable_values), len(tf_variables)))
        for value, tf_variable in zip(variable_values, tf_variables):
            session.run(tf_variable.assign(value))
    except ValueError as ex:
        # TODO: maybe raise custom exception
        raise ValueError("""Tried to load network file %s with different architecture from the in memory network.
Error was %s
Either delete the network file to train a new network from scratch or change the in memory network to match that dimensions of the one in the file""" % (file_path, ex))

def main(argv):
    #with tf.Session() as session:
    session = tf.Session()
    session.run(tf.global_variables_initializer())
    input_layer, output_layer, variables = create_network(36, ((100), (100), (100)), 36)

    load_network(session, variables, "testnetwork2P.p")
    
    input_board = argv[1:len(argv)] #input from the command line
    
    board_state = np.zeros((36,), dtype=int)

    for i in range(len(input_board)):
        board_state[i] = input_board[i]

    board_state = np.reshape(board_state, (6,6))
    tuple(map(tuple, board_state))
    #print(board_state)



    move = one_hot_to_move(get_stochastic_network_move_x(session, input_layer, output_layer, board_state, -1))
    board_state = apply_move(board_state, move, -1)

if __name__ == '__main__':
    main(sys.argv)
    #play_game(user_input, get_stochastic_network_move_x, session, log=True, board_size=6, winning_length=5) #board_size=10, winning_length=4