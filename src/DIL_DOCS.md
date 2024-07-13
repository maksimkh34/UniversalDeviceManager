# Device Interaction Language Guide
* DIL is scripting language, which means that it consists of commands and vars.
* Every new command starts with \r\n (newline), one command per line
## Commands
* `fastboot_reboot [mode]`
  Reboots device. Modes:
  * bootloader (fastboot)
  * recovery
  Empty mode means reboot into system.
* `fastboot_check_bl`
  Determines whether the bootloader is locked. Result will be displayed in logs
* `fastboot_flash [partition] [path to image]`
  Flashes image to partition using fastboot.
* `wait_for_bl`
  Waiting for selected device to boot into bootloader (fastboot)
  
## Vars
* `%cwd%` - Current working directory *(where is running executable located)*
  
Example:
```
fastboot_flash boot %cwd%\boot.img
```
