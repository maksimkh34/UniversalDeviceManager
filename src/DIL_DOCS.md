# Device Interaction Language Guide
* DIL is scripting language, which means that it consists of commands and vars.
* All vars should start with `$` char
* To access var value just write its name (that's why they should start with `$`)
* Comment lines starts with `#`
* Every new command starts with `\r\n` (newline), one command per line (except `py_exec`)
## Commands
* `fastboot_reboot (mode)` \\\\ `fr (mode)`

  Reboots device. Modes:
  * bootloader (fastboot)
  * recovery
  * EDL
	
  Empty mode means reboot into system.

  
* `fastboot_check_bl` \\\\ `fc`

  Determines whether the bootloader is locked. Result will be displayed in logs.

* `select (id)`

  Sets selected device's id to `(id)`

* `flash_rom (type) (path)`
	
  Flashes rom to device (like MiFlash do)

  * `type`
	* `-f` - flash and wipe
	* `-l` - flash and lock
	* `-k` - keep user files
	* `-d` - do not flash (do not pass in code)

* `sideload (file)` \\\\ `sl`

  Sideloads provided zip file to device.

  
* `fastboot_flash (partition) {flags} (path to image)` \\\\ `ff (partition) (path to image)`

  Flashes image to partition using fastboot.

  	* flag `-dt` => `--disable-verity`
  	* flag `-df` => `--disable-verification`

  
* `wait_for_bl` \\\\ `wb`

  Waiting for selected device to boot into bootloader (fastboot)

* `wait_for_recovery` \\\\ `wr`

  Waiting for selected device to boot into recovery (fastboot)

  
* `iget (path) (url)`
  
	* flag `--overwrite-if-exists` (or `-oie`) overwrites download file if exists

  Downloads file from url to path.

  
* `msg`

  Displays all the text from `msg` to end of the line.

  
* `wait_win` \\\\ `ww`

  Displays all the text from `msg` to end of the line.

  
* `write_var ($var_name)` \\\\ `wv`

  Writes var value.

  
* `log`

  Displays all the text from `log` to end of the line in new window.

  
* `rem (file)`

  Removes `file`.

  
* `unzip (file) (path)`

  Unzips `file` to `path`.

  
* `cmdexec (wd) (execCmd)`

  Executes `execCmd` with working directory `wd`.

* `adb_backup (block) (output path)`

  Backups provided block to output path on PC **(use with %getblock ...%)**

* `py_exec (cmd)`

  Executes python script.
```
py_exec script --args
{
  print
  write (msg)
  end
}
```
* `print` - prints script output to logs
* `display` - displays script output in new window
* `write (msg)` - writes msg to script
* `write_var ($var_name) (value)` - `write_var` during python execution
* `write_var_input ($var_name)` - writes python output to var
* `end` - sends end_wait signal to script *(InteractionService)*
  
## Vars
* `%cwd%` - Current working directory *(where is running executable located)*
  
Example:
```
fastboot_flash boot %cwd%\boot.img
```

* `%askuser: [msg]%` - User input (displays dialog asking for user input with msg displayed) 
  
Example:
```
msg %askuser: [Input text will be displayed in the next window]%
```

* `%sid%` - Selected device ID
  
Example:
```
msg %sid%
```

* `%pyexecutable%` - returns python.exe path
  
Example:
```
cmdexec %cwd% %pyexecutable% exit()
```

* `%getblock partition%` - returns block locaton of partition by name
  
Example:
```
msg %getblock recovery%
```

## If statement
```
if (expr1)=(expr2)
begin
	(code if expression is true)
else
	(code if expression is false)
end
```
* `expr1`'s and `expr2`'s string representations will be matched.
* Next to expression there should be minimum of 3 lines: `begin`, `else` and `end`.
* If needed, place your DIL code between those keywords.
* **Do not** separate `expr1`, `=` and `expr2`! It must be exactly `expr1=expr2`
* But remember. you still can use spaces inside `expr1` and `expr2`
