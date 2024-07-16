import sys


def ir_wait_for_input():
    if input() == "$$end_wait$$":
        return
    else:
        raise Exception("Got input while waiting for end_wait")


def ir_write(msg):
    sys.stdout.write(msg + '\n')


def ir_read_input():
    return input()
