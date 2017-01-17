# ELEC374 MiniSRC Assembler / MIF Generator

A simple assembler program for the mini SRC computer system. It does not have support for any custom instructions, however if you clone this repository you can add them to the InstructionParser.cs pretty easily. It supports windows natively, but will probably run on any *nix systems (Mac, Linux, etc.) with the mono framework

!["Screenshot of the program"](https://raw.githubusercontent.com/mitchellwaite/MiniSRC-Assembler/master/Screenshot.png)

#Downloads
* [Download the minisrc assembler here](https://github.com/mitchellwaite/MiniSRC-Assembler/raw/master/MiniSRC_ASM/MiniSRC_ASM_CMD/bin/Debug/minisrc_asm.exe)
* [Download the MiniSRC ISA spec here](https://github.com/mitchellwaite/MiniSRC-Assembler/raw/master/CPU_Spec.pdf)

#How to run

Windows: minisrc_asm.exe [file] [output] [size] [options]

Mac/Linux *(you must install the mono framework)*: mono minisrc_asm.exe [file] [output] [options]


[file] is the path to your input .asm/.s file, containing MiniSRC Assembly Code

[output] is the desired path to the output file, which is an Altera format MIF. You can convert it to Intel HEX format in Quartus, if desired

[size] is the desired size in (32 bit) words of memory.

[options] are the requested *case sensitive* display options, which include:

-showfile shows the output MIF in the console window

-noinfo hides any informational messages

-nowarn hides any warning messages

If any warnings occur, the program may *still* attempt to generate a MIF. The program will probably not crash, but do be aware that *your MIF file may be bad*.

#Assembly Syntax Improvements

-Specifying a subroutine is done on its own line, using an @ symbol. *Never place this directly before an ORG directive.* Currently subroutines can't be called directly, but this doesn't matter since the instruction set doesn't support it. If you want to extend the instruction set in that way, a table is created in FileParser in which you can use to make a second pass over the assembly file.

-To specify a hexadecimal constant, you can use '0x' or '$' interchangably

-To specify a data constant in memory, you can use an '&' symbol, followed by hexadecimal or decimal constants (Max 32 bit!). For clarity, it's recommended to do this *directly after* an ORG directive, but it isn't required.
