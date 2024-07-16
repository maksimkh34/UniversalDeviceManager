import os
import sys

sys.path.append(os.getcwd())
import interaction as i

i.write("Starting generator")
i.wait_for_input()
i.write("Enter your fastboot id")
fastboot_id = i.read()
i.write("Enter your device name")
device_name = i.read()
i.write(fastboot_id + ", " + device_name)
# read
# wait
# write
# write
# read
