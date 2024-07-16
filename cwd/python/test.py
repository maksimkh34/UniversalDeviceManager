import os
import sys

sys.path.append(os.getcwd())
from interaction import ir_write, ir_wait_for_input, ir_read_input

ir_write("Log 1")
pass
ir_write("Log 2")
pass
ir_wait_for_input()
pass
ir_write("waited")
pass
inp = ir_read_input()
ir_write(inp)
