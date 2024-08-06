import os
import sys


def wait_for_input():
    return input() == "$$end_wait$$"


def write(msg):
    sys.stdout.write(msg + '\n')


def execute_dil(msg):
    udm_dir = os.getenv('LOCALAPPDATA') + "\\UniversalDeviceManager"
    os.mkdir(udm_dir)
    f = open(udm_dir + "\\python_pipe.dil", "w")
    f.write(msg)
    f.close()


def read():
    return input()


def start_proc():
    sys.stdout.write("$$start_proc$$")


def end_proc():
    sys.stdout.write("$$end_proc$$")
