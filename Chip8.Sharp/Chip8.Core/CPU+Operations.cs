using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace Chip8.Core
{
    public partial class CPU
    {
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_CLS()
        {
            this.VideoBuffer.Clear();

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_RET()
        {
            if (this.SP == 0)
            {
                var message = "Illegal return operation, stack is empty.";
                Debug.Print(message);
                throw new StackOverflowException(message);
            }

            this.PC = this.Stack[this.SP - 1];
            this.SP--;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SYS_addr()
        {
            var message = $"Illegal call to RCA 1802 program: 0x{this.Opcode.ToString("X4", NumberFormatInfo.CurrentInfo)}";
            Debug.Print(message);
            throw new InvalidOperationException(message);
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_JP_addr(Opcode op)
        {
            // Now that we have stored the program counter, we can set it to the address NNN.
            // Remember, because we’re calling a subroutine at a specific address, 
            // you should not increase the program counter by two.

            if (op.NNN < 0x200)
            {
                var message = $"Illegal jump target: 0x{this.Opcode.ToString("X4", NumberFormatInfo.CurrentInfo)}";
                Debug.Print(message);
                throw new InvalidOperationException(message);
            }

            this.PC = op.NNN;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_CALL_addr(Opcode op)
        {
            if (this.SP > 0xF)
            {
                var message = "Illegal call operation, stack is full.";
                Debug.Print(message);
                throw new StackOverflowException(message);
            }

            // Because we will need to temporary jump to address NNN,
            // it means that we should store the current address of the program counter in the stack.
            this.Stack[this.SP] = this.PC;

            //  After storing the value of the program counter in the stack,
            // increase the stack pointer to prevent overwriting the current stack. 
            ++this.SP;

            // Now that we have stored the program counter, we can set it to the address NNN.
            // Remember, because we’re calling a subroutine at a specific address, 
            // you should not increase the program counter by two.
            this.PC = op.NNN;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SE_Vx_byte(Opcode op, byte vx)
        {
            if (vx == op.NN)
            {
                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                // (in which case you need to store the program counter in the stack).
                // If the next opcode should be skipped, increase the program counter by four.
                this.PC += 4;
            }
            else
            {
                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                // (in which case you need to store the program counter in the stack).
                // If the next opcode should be skipped, increase the program counter by four.
                this.PC += 2;
            }
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SNE_Vx_byte(Opcode op, byte vx)
        {
            if (vx != op.NN)
            {
                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                // (in which case you need to store the program counter in the stack).
                // If the next opcode should be skipped, increase the program counter by four.
                this.PC += 4;
            }
            else
            {
                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                // (in which case you need to store the program counter in the stack).
                // If the next opcode should be skipped, increase the program counter by four.
                this.PC += 2;
            }
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SE_Vx_Vy(Opcode op, byte vx, byte vy)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("SE Vx, Vy operation should not use X bigger than 0xF");
            }

            if (op.Y > 0xF)
            {
                throw new InvalidOperationException("LD Vx, Vy operation should not use Y bigger than 0xF");
            }

            if (vx == vy)
            {
                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                // (in which case you need to store the program counter in the stack).
                // If the next opcode should be skipped, increase the program counter by four.
                this.PC += 4;
            }
            else
            {
                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                // (in which case you need to store the program counter in the stack).
                // If the next opcode should be skipped, increase the program counter by four.
                this.PC += 2;
            }
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_Vx_byte(Opcode op)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("LD Vx, byte operation should not use X bigger than 0xF");
            }

            this.V[op.X] = op.NN;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_ADD_Vx_byte(Opcode op)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("ADD Vx, byte operation should not use X bigger than 0xF");
            }

            this.V[op.X] += op.NN;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_Vx_Vy(Opcode op, byte vy)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("LD Vx, Vy operation should not use X bigger than 0xF");
            }

            if (op.Y > 0xF)
            {
                throw new InvalidOperationException("LD Vx, Vy operation should not use Y bigger than 0xF");
            }

            this.V[op.X] = vy;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_OR_Vx_Vy(Opcode op, byte vy)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("OR Vx, Vy operation should not use X bigger than 0xF");
            }

            if (op.Y > 0xF)
            {
                throw new InvalidOperationException("OR Vx, Vy operation should not use Y bigger than 0xF");
            }

            this.V[op.X] |= vy;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_AND_Vx_Vy(Opcode op, byte vy)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("AND Vx, Vy operation should not use X bigger than 0xF");
            }

            if (op.Y > 0xF)
            {
                throw new InvalidOperationException("AND Vx, Vy operation should not use Y bigger than 0xF");
            }

            this.V[op.X] &= vy;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_XOR_Vx_Vy(Opcode op, byte vy)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("XOR Vx, Vy operation should not use X bigger than 0xF");
            }

            if (op.Y > 0xF)
            {
                throw new InvalidOperationException("XOR Vx, Vy operation should not use Y bigger than 0xF");
            }

            this.V[op.X] ^= vy;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_ADD_Vx_Vy(Opcode op, byte vx, byte vy)
        {
            if (op.X >= 0xF)
            {
                throw new InvalidOperationException("ADD Vx, Vy operation should not use X as 0xF or bigger (the carry flag register)");
            }

            if (op.Y >= 0xF)
            {
                throw new InvalidOperationException("ADD Vx, Vy operation should not use Y as 0xF or bigger (the carry flag register)");
            }

            if (vy > (0xFF - vx))
            {
                this.V[op.X] += vy;

                // Carry
                // Because the register can only store values from 0 to 255 (8 bit value),
                // it means that if the sum of VX and VY is larger than 255,
                // it can’t be stored in the register (or actually it starts counting from 0 again).
                // If the sum of VX and VY is larger than 255, 
                // we use the carry flag to let the system know that the total sum of both values was indeed larger than 255. 
                this.V[0xF] = 1;
            }
            else
            {
                this.V[op.X] += vy;

                this.V[0xF] = 0;
            }

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SUB_Vx_Vy(Opcode op, byte vx, byte vy)
        {
            if (op.X >= 0xF)
            {
                throw new InvalidOperationException("SUB Vx, Vy operation should not use X as 0xF or bigger (the carry flag register)");
            }

            if (op.Y >= 0xF)
            {
                throw new InvalidOperationException("SUB Vx, Vy operation should not use Y as 0xF or bigger (the carry flag register)");
            }

            if (vx > vy)
            {
                this.V[op.X] -= vy;

                // Carry
                // Because the register can only store values from 0 to 255 (8 bit value),
                // it means that if the sum of VX and VY is larger than 255,
                // it can’t be stored in the register (or actually it starts counting from 0 again).
                // If the sum of VX and VY is larger than 255, 
                // we use the carry flag to let the system know that the total sum of both values was indeed larger than 255. 
                this.V[0xF] = 1;
            }
            else
            {
                this.V[op.X] -= vy;

                this.V[0xF] = 0;
            }

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SHR_Vx_Vy(Opcode op, byte vx)
        {
            if (op.X >= 0xF)
            {
                throw new InvalidOperationException("SHR Vx, {Vy} operation should not use X as 0xF or bigger (the carry flag register)");
            }

            if (op.Y >= 0xF)
            {
                throw new InvalidOperationException("SHR Vx, {Vy} operation should not use Y as 0xF or bigger (the carry flag register)");
            }

            this.V[0xF] = unchecked((byte)(vx & 0x01));
            this.V[op.X] >>= 1;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SUBN_Vx_Vy(Opcode op, byte vx, byte vy)
        {
            if (op.X >= 0xF)
            {
                throw new InvalidOperationException("SUBN Vx, Vy operation should not use X as 0xF or bigger (the carry flag register)");
            }

            if (op.Y >= 0xF)
            {
                throw new InvalidOperationException("SUBN Vx, Vy operation should not use Y as 0xF or bigger (the carry flag register)");
            }

            if (vy > vx)
            {
                this.V[op.X] = unchecked((byte)(vy - vx));

                // Carry
                // Because the register can only store values from 0 to 255 (8 bit value),
                // it means that if the sum of VX and VY is larger than 255,
                // it can’t be stored in the register (or actually it starts counting from 0 again).
                // If the sum of VX and VY is larger than 255, 
                // we use the carry flag to let the system know that the total sum of both values was indeed larger than 255. 
                this.V[0xF] = 1;
            }
            else
            {
                this.V[op.X] = unchecked((byte)(vy - vx));

                this.V[0xF] = 0;
            }

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SHL_Vx_Vy(Opcode op, byte vx)
        {
            if (op.X >= 0xF)
            {
                throw new InvalidOperationException("SUBN Vx, Vy operation should not use X as 0xF or bigger (the carry flag register)");
            }

            if (op.Y >= 0xF)
            {
                throw new InvalidOperationException("SUBN Vx, Vy operation should not use Y as 0xF or bigger (the carry flag register)");
            }

            this.V[0xF] = unchecked((byte)((vx & 0x80) >> 7));
            this.V[op.X] <<= 1;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SNE_Vx_Vy(Opcode op, byte vx, byte vy)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("SNE Vx, Vy operation should not use X bigger than 0xF");
            }

            if (op.Y > 0xF)
            {
                throw new InvalidOperationException("SNE Vx, Vy operation should not use Y bigger than 0xF");
            }

            if (vx != vy)
            {
                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                // (in which case you need to store the program counter in the stack).
                // If the next opcode should be skipped, increase the program counter by four.
                this.PC += 4;
            }
            else
            {
                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                // (in which case you need to store the program counter in the stack).
                // If the next opcode should be skipped, increase the program counter by four.
                this.PC += 2;
            }
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_I_addr(Opcode op)
        {
            this.I = op.NNN;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_JP_V0_addr(Opcode op)
        {
            // Now that we have stored the program counter, we can set it to the address NNN.
            // Remember, because we’re calling a subroutine at a specific address, 
            // you should not increase the program counter by two.
            var dest = this.V[0] + op.NNN;

            if (dest < 0x200)
            {
                var message = $"Illegal jump target: 0x{dest.ToString("X4", NumberFormatInfo.CurrentInfo)} => 0x{this.Opcode.ToString("X4", NumberFormatInfo.CurrentInfo)} + this.V[0] (0x{this.V[0].ToString("X2", NumberFormatInfo.CurrentInfo)})";
                Debug.Print(message);
                throw new InvalidOperationException(message);
            }

            if (dest > 0xFFF)
            {
                var message = $"Illegal jump target: 0x{dest.ToString("X4", NumberFormatInfo.CurrentInfo)} => 0x{this.Opcode.ToString("X4", NumberFormatInfo.CurrentInfo)} + this.V[0] (0x{this.V[0].ToString("X2", NumberFormatInfo.CurrentInfo)})";
                Debug.Print(message);
                throw new InvalidOperationException(message);
            }

            this.PC = unchecked((ushort)dest);
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_RND_Vx_byte(Opcode op)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("RND Vx, byte operation should not use X bigger than 0xF");
            }

            this.V[op.X] = unchecked((byte)(this.rand.Next(0, 0x100) & op.NN));

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_DRW_Vx_Vy_nibble(Opcode op, byte vx, byte vy)
        {
            // Fetch the position and height of the sprite
            ushort height = op.N;

            ushort pixel;

            // Reset register VF
            this.V[0xF] = 0;

            // Loop over each row
            for (int row = 0; row < height; row++)
            {
                // Fetch the pixel value from the memory starting at location this.I
                pixel = this.Memory[this.I + row];

                // Loop over 8 bits (columns) of one row
                for (int column = 0; column < 8; column++)
                {
                    // Check if the current evaluated pixel is set to 1
                    // Note that (0x80 >> column) scan through the byte, one bit at the time
                    if ((pixel & (0x80 >> column)) != 0)
                    {
                        // Check if the pixel on the display is set to 1.
                        //  If it is set, we need to register the collision by setting the VF register
                        if (this.VideoBuffer[vx + column, vy + row])
                        {
                            this.V[0xF] = 1;
                        }

                        // Set the pixel value by using XOR
                        this.VideoBuffer[vx + column, vy + row] ^= true;
                    }
                }
            }

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SKP_Vx(Opcode op, byte vx)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("SKP Vx operation should not use X bigger than 0xF");
            }

            if (vx > 0xF)
            {
                throw new InvalidOperationException("SKP Vx operation should not use VX value bigger than 0xF");
            }

            if (this.Keys[vx] != 0)
            {
                this.PC += 4;
            }
            else
            {
                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                // (in which case you need to store the program counter in the stack).
                // If the next opcode should be skipped, increase the program counter by four.
                this.PC += 2;
            }
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_SKNP_Vx(Opcode op, byte vx)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("SKP Vx operation should not use X bigger than 0xF");
            }

            if (vx > 0xF)
            {
                throw new InvalidOperationException("SKP Vx operation should not use VX value bigger than 0xF");
            }

            if (this.Keys[vx] == 0)
            {
                this.PC += 4;
            }
            else
            {
                // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
                // This is true unless you jump to a certain address in the memory or if you call a subroutine
                // (in which case you need to store the program counter in the stack).
                // If the next opcode should be skipped, increase the program counter by four.
                this.PC += 2;
            }
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_Vx_DT(Opcode op)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("LD Vx, this.DT operation should not use X bigger than 0xF");
            }

            this.V[op.X] = this.DT;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_Vx_K(Opcode op)
        {
            var keyPressed = false;

            while (!keyPressed)
            {
                for (int index = 0; index < this.Keys.Length; index++)
                {
                    var keyValue = this.Keys[index];
                    keyPressed |= keyValue > 0;

                    if (keyPressed)
                    {
                        this.V[op.X] = unchecked((byte)index);
                        break;
                    }
                }

                Thread.Sleep(250);
            }

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_DT_Vx(Opcode op, byte vx)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("LD this.DT, Vx operation should not use X bigger than 0xF");
            }

            this.DT = vx;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_ST_Vx(Opcode op, byte vx)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("LD this.ST, Vx operation should not use X bigger than 0xF");
            }

            this.ST = vx;

            if (this.ST > 0)
            {
                this.OnStartSound?.Invoke(0, 500, this.ST);
            }

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_ADD_I_Vx(Opcode op, byte vx)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("ADD this.I, Vx operation should not use X bigger than 0xF");
            }

            if ((this.I + vx) > 0xFFF)
            {
                throw new InvalidOperationException("ADD this.I, Vx operation should not exceed 0xFFF");
            }

            this.I += vx;

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_F_Vx(Opcode op, byte vx)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("LD F, Vx operation should not use X bigger than 0xF");
            }

            if (vx > 0x0F)
            {
                throw new InvalidOperationException("LD F, Vx operation should not try to access sprites higher than 0x0F");
            }

            this.I = unchecked((ushort)(vx * 5));

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_B_Vx(Opcode op, byte vx)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("LD B, Vx operation should not use X bigger than 0xF");
            }

            if (this.I < 0x200)
            {
                throw new InvalidOperationException("LD B, Vx operation should be below 0x200");
            }

            if ((this.I + 2) > 0xFFF)
            {
                throw new InvalidOperationException("LD B, Vx operation should not exceed address 0xFFF");
            }

            this.Memory[this.I] = unchecked((byte)(vx / 100));
            this.Memory[this.I + 1] = unchecked((byte)((vx / 10) % 10));
            this.Memory[this.I + 2] = unchecked((byte)((vx % 100) % 10));

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_IPtr_Vx(Opcode op)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("LD [this.I], Vx operation should not use X bigger than 0xF");
            }

            var destAddr = this.I + op.X;

            if (destAddr > 0xFFF)
            {
                throw new InvalidOperationException("LD [this.I], Vx operation should not exceed 0xFFF");
            }

            if (destAddr < 0x200)
            {
                throw new InvalidOperationException("LD [this.I], Vx operation should be below 0x200");
            }

            for (int index = 0; index <= op.X; ++index)
            {
                this.Memory[this.I + index] = this.V[index];
            }

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Using op_ prefix for all CPU instructions methods")]
        internal void op_LD_Vx_IPtr(Opcode op)
        {
            if (op.X > 0xF)
            {
                throw new InvalidOperationException("LD Vx, [this.I] operation should not use X bigger than 0xF");
            }

            if ((this.I + op.X) > 0xFFF)
            {
                throw new InvalidOperationException("LD Vx, [this.I] operation should not exceed 0xFFF");
            }

            for (int index = 0; index <= op.X; ++index)
            {
                this.V[index] = this.Memory[this.I + index];
            }

            // Because every instruction is 2 bytes long, we need to increment the program counter by two after every executed opcode.
            // This is true unless you jump to a certain address in the memory or if you call a subroutine
            // (in which case you need to store the program counter in the stack).
            // If the next opcode should be skipped, increase the program counter by four.
            this.PC += 2;
        }
    }
}
