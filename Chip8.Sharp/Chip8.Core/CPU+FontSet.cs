﻿using System;

namespace Chip8.Core
{
    public partial class CPU
    {
        // Programs may also refer to a group of sprites representing the hexadecimal digits 0 through F.
        // These sprites are 5 bytes long, or 8x5 pixels.
        // The data should be stored in the interpreter area of Chip-8 memory (0x000 to 0x1FF). 
        internal readonly static byte[] CHIP8_FONTSET =
        {
            //**** 11110000
            //*  * 10010000
            //*  * 10010000
            //*  * 10010000
            //**** 11110000
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0

            // *   00100000
            //**   01100000
            // *   00100000
            // *   00100000
            //***  01110000
            0x20, 0x60, 0x20, 0x20, 0x70, // 1

            //**** 11110000
            //   * 00010000
            //**** 11110000
            //*    10000000
            //**** 11110000
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2

            //**** 11110000
            //   * 00010000
            //**** 11110000
            //   * 00010000
            //**** 11110000
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3

            //*  * 10010000
            //*  * 10010000
            //**** 11110000
            //   * 00010000
            //   * 00010000
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4

            //**** 11110000
            //*    10000000
            //**** 11110000
            //   * 00010000
            //**** 11110000
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5

            //**** 11110000
            //*    10000000
            //**** 11110000
            //*  * 10010000
            //**** 11110000
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6

            //**** 11110000
             //  * 00010000
             // *  00100000
             //*   01000000
             //*   01000000
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7

            //**** 11110000
            //*  * 10010000
            //**** 11110000
            //*  * 10010000
            //**** 11110000
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8

            //**** 11110000
            //*  * 10010000
            //**** 11110000
            //   * 00010000
            //**** 11110000
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9

            //**** 11110000
            //*  * 10010000
            //**** 11110000
            //*  * 10010000
            //*  * 10010000
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A

            //***  11100000
            //*  * 10010000
            //***  11100000
            //*  * 10010000
            //***  11100000
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B

            //**** 11110000
            //*    10000000
            //*    10000000
            //*    10000000
            //**** 11110000
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C

            //***  11100000
            //*  * 10010000
            //*  * 10010000
            //*  * 10010000
            //***  11100000
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D

            //**** 11110000
            //*    10000000
            //**** 11110000
            //*    10000000
            //**** 11110000
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E

            //**** 11110000
            //*    10000000
            //**** 11110000
            //*    10000000
            //*    10000000
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };
    }
}
