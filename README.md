# AutoMemOC
Automatic memory overclocking tool.


# Project Status
This project has been abandoned as a Windows update in mid-2020 broke MemTweakIt.

# Important Notes
* This program only works for Intel platforms and is optimised for 2 DIMM single rank configuration only.
* This program primarily tightens secondaries and tertiaries. tCL, tRAS and tCWL are untouched as they affect memory training.
* This program requires you to find a stable frequency and primary timings, and optionally stable loose secondary and tertiary timings. 
* This program will not modify voltages. A tighter timing may be stable at higher/lower voltages, but this program will not detect that. 

# Usage
* Download and install your choice of a memory tweaker from the supported list below.
* Download your choice of a memory testor from the supported list below.
* Download [AutoMemOC](https://github.com/KingFaris10/AutoMemOC/releases)
* Run AutoMemOC as administrator.
* If your system crashes due to unstable RAM settings, that's expected. Keep running AutoMemOC until the program tells you the discovered stable settings. Plug those numbers into your BIOS and you're done.


Note: If the program's behaviour starts acting up, try deleting config.txt and restart the process again.

## Command-Line Arguments
### Tweaker path
Argument: --tweak path

Description: The directory where the tweaker executable file is located.

### Memory test path
Argument: --test path

Description: The directory where the memory testing executable file is located.

### Verbose logging
Argument: --verbose

Aliases: -v

Description: Enable verbose logging for more detailed information on what the program is doing, including error information. This can be useful when reporting issues to me.

# Support
## Tweakers
### MemTweakIt
Download and install MemTweakIt.

* [Z390](https://dlcdnets.asus.com/pub/ASUS/mb/Utility/Mem_TweakIt_WIN10-64_V2.02.41.zip) - This may or may not work on your board, regardless of whether you have an ASUS board or not. This definitely works on ASUS Maximus XI series and works on my MSI MPG Z390I Gaming Edge AC.


Run this program at least once before running AutoMemOC to ensure the tweaker does not show any warning/confirmation screens on load and that it works with your board.

## Memory Tests
### TestMem5 (TM5)
* Download [TestMem5](http://testmem.tz.ru/tm5.rar) and ensure the directory is named TM5 or TestMem5.
* It is recommended to test using the [anta777](https://drive.google.com/file/d/1uegPn9ZuUoWxOssCP4PjMjGW9eC_1VJA/view) config. Run TestMem5 once, click 'Load config & exit' and load the linked configuration.
