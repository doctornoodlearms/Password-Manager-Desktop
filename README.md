# Password Manager Desktop

A password manager to easily generate passwords

## Command Line

If the program is opened from a command line then you can also pass in commands

- Test:
    - Used for debugging and just prints a message to the console
    - Syntax: -test

- Safe Mode
    - Forces the program to start in a safe mode to prevent it from saving anything
    - Syntax: -safe

- Get Password
    - Automatically genertates a password after loading while also loading into safe mode
    - Syntax: -pass -master [Master Password] -key [Password Key]

## Change Log

### v1.1.3
- Updated to Godot 3.5

### v1.1.2
- General improvements to prevent memory leaks

### v1.1.1
- Removed enviornment variable because of jank

### v1.1.0
- Added Priority to saved passwords
- Added command line support
- Added an enviornment variable to open the program
- Added a safe mode