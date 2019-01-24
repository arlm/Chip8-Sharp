using System;
using System.Collections;
using Chip8.Core;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chip8.Tests.Utils;
using System.Threading.Tasks;

namespace Chip8.Tests
{
    [TestFixture]
    [Parallelizable]
    public class TestCPU
    {
        private static Random rnd = new Random();

        [Test]
        [Parallelizable]
        [Category("Functional Tests")]
        public void InitialState()
        {
            var emptyMemory = new byte[0x1000];
            var cpu = new CPU();

            Assert.AreEqual(0x1000, cpu.memory.Length, "Memory should be exactly 4KiB long.");
            Assert.AreEqual(16, cpu.V.Length, "The CPU must have exactl 16 V registers.");
            CollectionAssert.AllItemsAreInstancesOfType(cpu.V, typeof(byte), "The CPU must have 8-bit registers.");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            Assert.Zero(cpu.I, $"Register I should be zero.");
            Assert.IsInstanceOf<ushort>(cpu.I, $"Register I have 16-bits.");

            Assert.Zero(cpu.DT, $"Register DT should be zero.");
            Assert.IsInstanceOf<byte>(cpu.DT, $"Register DT have 8-bits.");

            Assert.Zero(cpu.ST, $"Register ST should be zero.");
            Assert.IsInstanceOf<byte>(cpu.ST, $"Register ST have 8-bits.");

            Assert.Zero(cpu.SP, $"Register SP should be zero.");
            Assert.AreEqual(16, cpu.stack.Length, "The stack must have exactl 16 positions.");
            CollectionAssert.AllItemsAreInstancesOfType(cpu.stack, typeof(ushort), "The stack must have have 16-bit elements.");

            for (int index = 0; index < cpu.stack.Length; index++)
            {
                Assert.Zero(cpu.stack[index], $"Stack position {index:X2} should be zero.");
            }

            Assert.AreEqual(16, cpu.keys.Length, "The CPU must have exactly 16 keys.");
            CollectionAssert.AllItemsAreInstancesOfType(cpu.keys, typeof(byte), "The keys registers must be 8-bit elements.");

            for (int index = 0; index < cpu.keys.Length; index++)
            {
                Assert.Zero(cpu.keys[index], $"Key {index:X2} should be zero.");
            }

            byte[] character = new byte[5];

            Array.Copy(cpu.memory, 0, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x90, 0x90, 0x90, 0xF0 }, "Problem with character 0");

            Array.Copy(cpu.memory, 5, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0x20, 0x60, 0x20, 0x20, 0x70 }, "Problem with character 1");

            Array.Copy(cpu.memory, 10, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x10, 0xF0, 0x80, 0xF0 }, "Problem with character 2");

            Array.Copy(cpu.memory, 15, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x10, 0xF0, 0x10, 0xF0 }, "Problem with character 3");

            Array.Copy(cpu.memory, 20, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0x90, 0x90, 0xF0, 0x10, 0x10 }, "Problem with character 4");

            Array.Copy(cpu.memory, 25, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x80, 0xF0, 0x10, 0xF0 }, "Problem with character 5");

            Array.Copy(cpu.memory, 30, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x80, 0xF0, 0x90, 0xF0 }, "Problem with character 6");

            Array.Copy(cpu.memory, 35, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x10, 0x20, 0x40, 0x40 }, "Problem with character 7");

            Array.Copy(cpu.memory, 40, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x90, 0xF0, 0x90, 0xF0 }, "Problem with character 8");

            Array.Copy(cpu.memory, 45, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x90, 0xF0, 0x10, 0xF0 }, "Problem with character 9");

            Array.Copy(cpu.memory, 50, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x90, 0xF0, 0x90, 0x90 }, "Problem with character A");

            Array.Copy(cpu.memory, 55, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xE0, 0x90, 0xE0, 0x90, 0xE0 }, "Problem with character B");

            Array.Copy(cpu.memory, 60, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x80, 0x80, 0x80, 0xF0 }, "Problem with character C");

            Array.Copy(cpu.memory, 65, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xE0, 0x90, 0x90, 0x90, 0xE0 }, "Problem with character D");

            Array.Copy(cpu.memory, 70, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x80, 0xF0, 0x80, 0xF0 }, "Problem with character E");

            Array.Copy(cpu.memory, 75, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x80, 0xF0, 0x80, 0x80 }, "Problem with character F");

            Assert.Zero(cpu.opcode, $"Opcode should be zero.");
            Assert.Zero(cpu.opcode, $"Register SP should be zero.");
            Assert.IsInstanceOf<ushort>(cpu.opcode, $"Register DT have 16-bits.");

            Assert.AreEqual(0x200, cpu.PC, $"Register PC should be 0x200.");
            Assert.IsInstanceOf<ushort>(cpu.PC, $"Register DT have 16-bits.");

            cpu.LoadMemory(emptyMemory);

            Array.Copy(cpu.memory, 0, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x90, 0x90, 0x90, 0xF0 }, "Problem with character 0");

            Array.Copy(cpu.memory, 5, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0x20, 0x60, 0x20, 0x20, 0x70 }, "Problem with character 1");

            Array.Copy(cpu.memory, 10, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x10, 0xF0, 0x80, 0xF0 }, "Problem with character 2");

            Array.Copy(cpu.memory, 15, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x10, 0xF0, 0x10, 0xF0 }, "Problem with character 3");

            Array.Copy(cpu.memory, 20, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0x90, 0x90, 0xF0, 0x10, 0x10 }, "Problem with character 4");

            Array.Copy(cpu.memory, 25, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x80, 0xF0, 0x10, 0xF0 }, "Problem with character 5");

            Array.Copy(cpu.memory, 30, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x80, 0xF0, 0x90, 0xF0 }, "Problem with character 6");

            Array.Copy(cpu.memory, 35, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x10, 0x20, 0x40, 0x40 }, "Problem with character 7");

            Array.Copy(cpu.memory, 40, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x90, 0xF0, 0x90, 0xF0 }, "Problem with character 8");

            Array.Copy(cpu.memory, 45, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x90, 0xF0, 0x10, 0xF0 }, "Problem with character 9");

            Array.Copy(cpu.memory, 50, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x90, 0xF0, 0x90, 0x90 }, "Problem with character A");

            Array.Copy(cpu.memory, 55, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xE0, 0x90, 0xE0, 0x90, 0xE0 }, "Problem with character B");

            Array.Copy(cpu.memory, 60, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x80, 0x80, 0x80, 0xF0 }, "Problem with character C");

            Array.Copy(cpu.memory, 65, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xE0, 0x90, 0x90, 0x90, 0xE0 }, "Problem with character D");

            Array.Copy(cpu.memory, 70, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x80, 0xF0, 0x80, 0xF0 }, "Problem with character E");

            Array.Copy(cpu.memory, 75, character, 0, 5);
            CollectionAssert.AreEqual(character, new byte[] { 0xF0, 0x80, 0xF0, 0x80, 0x80 }, "Problem with character F");

            int address = 0x000;

            for (int index = address; index < CPU.CHIP8_FONTSET.Length; index++, address++)
            {
                Assert.AreEqual(CPU.CHIP8_FONTSET[index], cpu.memory[index], $"Memory address {index:X2} should have the CHIP-8 Fontset.");
            }

            for (int index = address; index < cpu.memory.Length; index++)
            {
                Assert.Zero(cpu.memory[index], $"Memory address {index:X2} should be zero.");
            }
        }

        [Test]
        [Parallelizable]
        [Category("Functional Tests")]
        public void SubRoutineStack()
        {
            var emptyMemory = new byte[0x1000];

            #region data initialization
            emptyMemory[0x200] = 0x23;
            emptyMemory[0x201] = 0x00;

            emptyMemory[0x202] = 0x00;
            emptyMemory[0x203] = 0xEE;

            emptyMemory[0x300] = 0x24;
            emptyMemory[0x301] = 0x00;

            emptyMemory[0x302] = 0x00;
            emptyMemory[0x303] = 0xEE;

            emptyMemory[0x400] = 0x25;
            emptyMemory[0x401] = 0x00;

            emptyMemory[0x402] = 0x00;
            emptyMemory[0x403] = 0xEE;

            emptyMemory[0x500] = 0x26;
            emptyMemory[0x501] = 0x00;

            emptyMemory[0x502] = 0x00;
            emptyMemory[0x503] = 0xEE;

            emptyMemory[0x600] = 0x27;
            emptyMemory[0x601] = 0x00;

            emptyMemory[0x602] = 0x00;
            emptyMemory[0x603] = 0xEE;

            emptyMemory[0x700] = 0x28;
            emptyMemory[0x701] = 0x00;

            emptyMemory[0x702] = 0x00;
            emptyMemory[0x703] = 0xEE;

            emptyMemory[0x800] = 0x29;
            emptyMemory[0x801] = 0x00;

            emptyMemory[0x802] = 0x00;
            emptyMemory[0x803] = 0xEE;

            emptyMemory[0x900] = 0x2A;
            emptyMemory[0x901] = 0x00;

            emptyMemory[0x902] = 0x00;
            emptyMemory[0x903] = 0xEE;

            emptyMemory[0xA00] = 0x2B;
            emptyMemory[0xA01] = 0x00;

            emptyMemory[0xA02] = 0x00;
            emptyMemory[0xA03] = 0xEE;

            emptyMemory[0xB00] = 0x2C;
            emptyMemory[0xB01] = 0x00;

            emptyMemory[0xB02] = 0x00;
            emptyMemory[0xB03] = 0xEE;

            emptyMemory[0xC00] = 0x2D;
            emptyMemory[0xC01] = 0x00;

            emptyMemory[0xC02] = 0x00;
            emptyMemory[0xC03] = 0xEE;

            emptyMemory[0xD00] = 0x2E;
            emptyMemory[0xD01] = 0x00;

            emptyMemory[0xD02] = 0x00;
            emptyMemory[0xD03] = 0xEE;

            emptyMemory[0xE00] = 0x2F;
            emptyMemory[0xE01] = 0x10;

            emptyMemory[0xE02] = 0x00;
            emptyMemory[0xE03] = 0xEE;

            emptyMemory[0xF10] = 0x2F;
            emptyMemory[0xF11] = 0x20;

            emptyMemory[0xF12] = 0x00;
            emptyMemory[0xF13] = 0xEE;

            emptyMemory[0xF20] = 0x2F;
            emptyMemory[0xF21] = 0x30;

            emptyMemory[0xF22] = 0x00;
            emptyMemory[0xF23] = 0xEE;

            emptyMemory[0xF30] = 0x2F;
            emptyMemory[0xF31] = 0x40;

            emptyMemory[0xF32] = 0x00;
            emptyMemory[0xF33] = 0xEE;

            emptyMemory[0xF40] = 0x00;
            emptyMemory[0xF41] = 0xEE;
            #endregion

            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");
            Assert.Zero(cpu.SP, "SP Register should be zero");
            CollectionAssert.AreEqual(new ushort[16], cpu.stack, "Stack should be empty");

            var initialStack = new ushort[]
            {
                0x200, 0x300, 0x400, 0x500, 0x600, 0x700, 0x800, 0x900,
                0xA00, 0xB00, 0xC00, 0xD00, 0xE00, 0xF10, 0xF20, 0xF30
            };

            cpu.EmulateCycle();

            Assert.AreEqual(0x2300, cpu.opcode, "Opcode shoud be 0x2206 - CALL 0x300");
            Assert.AreEqual(0x300, cpu.PC, "PC Register should be at 0x300");
            Assert.AreEqual(1, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x200, cpu.stack[cpu.SP - 1], "Current stack should point to 0x200");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2400, cpu.opcode, "Opcode shoud be 0x2400 - CALL 0x400");
            Assert.AreEqual(0x400, cpu.PC, "PC Register should be at 0x400");
            Assert.AreEqual(2, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x300, cpu.stack[cpu.SP - 1], "Current stack should point to 0x300");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2500, cpu.opcode, "Opcode shoud be 0x2500 - CALL 0x500");
            Assert.AreEqual(0x500, cpu.PC, "PC Register should be at 0x500");
            Assert.AreEqual(3, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x400, cpu.stack[cpu.SP - 1], "Current stack should point to 0x400");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2600, cpu.opcode, "Opcode shoud be 0x2600 - CALL 0x600");
            Assert.AreEqual(0x600, cpu.PC, "PC Register should be at 0x600");
            Assert.AreEqual(4, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x500, cpu.stack[cpu.SP - 1], "Current stack should point to 0x500");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2700, cpu.opcode, "Opcode shoud be 0x2700 - CALL 0x700");
            Assert.AreEqual(0x700, cpu.PC, "PC Register should be at 0x700");
            Assert.AreEqual(5, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x600, cpu.stack[cpu.SP - 1], "Current stack should point to 0x600");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2800, cpu.opcode, "Opcode shoud be 0x2800 - CALL 0x800");
            Assert.AreEqual(0x800, cpu.PC, "PC Register should be at 0x800");
            Assert.AreEqual(6, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x700, cpu.stack[cpu.SP - 1], "Current stack should point to 0x700");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2900, cpu.opcode, "Opcode shoud be 0x2900 - CALL 0x900");
            Assert.AreEqual(0x900, cpu.PC, "PC Register should be at 0x900");
            Assert.AreEqual(7, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x800, cpu.stack[cpu.SP - 1], "Current stack should point to 0x800");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2A00, cpu.opcode, "Opcode shoud be 0x2A00 - CALL 0xA00");
            Assert.AreEqual(0xA00, cpu.PC, "PC Register should be at 0xA00");
            Assert.AreEqual(8, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x900, cpu.stack[cpu.SP - 1], "Current stack should point to 0x900");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2B00, cpu.opcode, "Opcode shoud be 0x2B00 - CALL 0xB00");
            Assert.AreEqual(0xB00, cpu.PC, "PC Register should be at 0xB00");
            Assert.AreEqual(9, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xA00, cpu.stack[cpu.SP - 1], "Current stack should point to 0xA00");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2C00, cpu.opcode, "Opcode shoud be 0x2C00 - CALL 0xC00");
            Assert.AreEqual(0xC00, cpu.PC, "PC Register should be at 0xC00");
            Assert.AreEqual(10, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xB00, cpu.stack[cpu.SP - 1], "Current stack should point to 0xB00");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2D00, cpu.opcode, "Opcode shoud be 0x2D00 - CALL 0xD00");
            Assert.AreEqual(0xD00, cpu.PC, "PC Register should be at 0xD00");
            Assert.AreEqual(11, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xC00, cpu.stack[cpu.SP - 1], "Current stack should point to 0xC00");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2E00, cpu.opcode, "Opcode shoud be 0x2E00 - CALL 0xE00");
            Assert.AreEqual(0xE00, cpu.PC, "PC Register should be at 0xE00");
            Assert.AreEqual(12, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xD00, cpu.stack[cpu.SP - 1], "Current stack should point to 0xD00");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2F10, cpu.opcode, "Opcode shoud be 0x2F10 - CALL 0xF10");
            Assert.AreEqual(0xF10, cpu.PC, "PC Register should be at 0xF10");
            Assert.AreEqual(13, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xE00, cpu.stack[cpu.SP - 1], "Current stack should point to 0xE00");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2F20, cpu.opcode, "Opcode shoud be 0x2F20 - CALL 0xF20");
            Assert.AreEqual(0xF20, cpu.PC, "PC Register should be at 0xF20");
            Assert.AreEqual(14, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xF10, cpu.stack[cpu.SP - 1], "Current stack should point to 0xF10");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2F30, cpu.opcode, "Opcode shoud be 0x2F30 - CALL 0xF30");
            Assert.AreEqual(0xF30, cpu.PC, "PC Register should be at 0xF30");
            Assert.AreEqual(15, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xF20, cpu.stack[cpu.SP - 1], "Current stack should point to 0xF20");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2F40, cpu.opcode, "Opcode shoud be 0x2F40 - CALL 0xF40");
            Assert.AreEqual(0xF40, cpu.PC, "PC Register should be at 0xF40");
            Assert.AreEqual(16, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xF30, cpu.stack[cpu.SP - 1], "Current stack should point to 0xF30");

            CollectionAssert.AreEqual(initialStack, cpu.stack, "Stack should be full");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0xF32, cpu.PC, "PC Register should be at 0xF32");
            Assert.AreEqual(15, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xF20, cpu.stack[cpu.SP - 1], "Current stack should point to 0xF20");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0xF22, cpu.PC, "PC Register should be at 0xF22");
            Assert.AreEqual(14, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xF10, cpu.stack[cpu.SP - 1], "Current stack should point to 0xF10");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0xF12, cpu.PC, "PC Register should be at 0xF12");
            Assert.AreEqual(13, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xE00, cpu.stack[cpu.SP - 1], "Current stack should point to 0xE00");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0xE02, cpu.PC, "PC Register should be at 0xE02");
            Assert.AreEqual(12, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xD00, cpu.stack[cpu.SP - 1], "Current stack should point to 0xD00");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0xD02, cpu.PC, "PC Register should be at 0xD02");
            Assert.AreEqual(11, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xC00, cpu.stack[cpu.SP - 1], "Current stack should point to 0xC00");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0xC02, cpu.PC, "PC Register should be at 0xC02");
            Assert.AreEqual(10, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xB00, cpu.stack[cpu.SP - 1], "Current stack should point to 0xB00");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0xB02, cpu.PC, "PC Register should be at 0xB02");
            Assert.AreEqual(9, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0xA00, cpu.stack[cpu.SP - 1], "Current stack should point to 0xA00");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0xA02, cpu.PC, "PC Register should be at 0xA02");
            Assert.AreEqual(8, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x900, cpu.stack[cpu.SP - 1], "Current stack should point to 0x900");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x902, cpu.PC, "PC Register should be at 0x902");
            Assert.AreEqual(7, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x800, cpu.stack[cpu.SP - 1], "Current stack should point to 0x800");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x802, cpu.PC, "PC Register should be at 0x802");
            Assert.AreEqual(6, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x700, cpu.stack[cpu.SP - 1], "Current stack should point to 0x700");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x702, cpu.PC, "PC Register should be at 0x702");
            Assert.AreEqual(5, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x600, cpu.stack[cpu.SP - 1], "Current stack should point to 0x600");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x602, cpu.PC, "PC Register should be at 0x602");
            Assert.AreEqual(4, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x500, cpu.stack[cpu.SP - 1], "Current stack should point to 0x500");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x502, cpu.PC, "PC Register should be at 0x502");
            Assert.AreEqual(3, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x400, cpu.stack[cpu.SP - 1], "Current stack should point to 0x400");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x402, cpu.PC, "PC Register should be at 0x402");
            Assert.AreEqual(2, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x300, cpu.stack[cpu.SP - 1], "Current stack should point to 0x300");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x302, cpu.PC, "PC Register should be at 0x302");
            Assert.AreEqual(1, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x200, cpu.stack[cpu.SP - 1], "Current stack should point to 0x200");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x202, cpu.PC, "PC Register should be at 0x202");
            Assert.AreEqual(0, cpu.SP, "PC Register should have incremented");

            Assert.Throws<StackOverflowException>(cpu.EmulateCycle, "Should throw a Stack Overflow Exception");
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SYS_addr()
        {
            var emptyMemory = new byte[0x1000];
            int index = 0x200;

            for (ushort data = 0x000; data <= 0x06FF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                if (low == 0xE0 || low == 0xEE) continue;

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (ushort counter = 0x200; counter <= 0xFFE; counter += 2)
            {
                cpu.PC = counter;
                Assert.Throws<InvalidOperationException>(cpu.EmulateCycle, $"Should throw a Invalid Operation Exception at 0x{cpu.PC:X2} [opcode: {cpu.opcode:X4}]");
            }

            index = 0x200;

            for (ushort data = 0x0700; data <= 0x0DFF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                if (low == 0xE0 || low == 0xEE) continue;

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            cpu.LoadMemory(emptyMemory);

            for (ushort counter = 0x200; counter <= 0xFFE; counter += 2)
            {
                cpu.PC = counter;
                Assert.Throws<InvalidOperationException>(cpu.EmulateCycle, $"Should throw a Invalid Operation Exception at 0x{cpu.PC:X2} [opcode: {cpu.opcode:X4}]");
            }

            index = 0x200;

            for (ushort data = 0x0E00; data <= 0x0FFF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                if (low == 0xE0 || low == 0xEE) continue;

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            cpu.LoadMemory(emptyMemory);

            for (ushort counter = 0x200; counter <= 0x5FF; counter += 2)
            {
                cpu.PC = counter;
                Assert.Throws<InvalidOperationException>(cpu.EmulateCycle, $"Should throw a Invalid Operation Exception at 0x{cpu.PC:X2} [opcode: {cpu.opcode:X4}]");
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_CLS()
        {
            var emptyMemory = new byte[0x1000];
            emptyMemory[0x200] = 0x00;
            emptyMemory[0x201] = 0xE0;

            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index < cpu.gfx.Length; index++)
            {
                Assert.Zero(cpu.gfx[index], $"Graphic memory address {index:X2} should be zero.");
            }

            for (int index = 0; index < cpu.gfx.Length; index++)
            {
                cpu.gfx[index] = unchecked((byte)rnd.Next(0, 2));
            }

            CollectionAssert.AreNotEqual(new byte[0x800], cpu.gfx, "Graphic memory address should not be blank.");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00E0, cpu.opcode, "Opcode shoud be 0x00E0 - CLS");
            Assert.AreEqual(0x202, cpu.PC, "PC Register should be at 0x202");

            for (int index = 0; index < cpu.gfx.Length; index++)
            {
                Assert.Zero(cpu.gfx[index], $"Graphic memory address {index:X2} should be zero.");
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_RET()
        {
            var emptyMemory = new byte[0x1000];
            #region data initialization
            emptyMemory[0x200] = 0x00;
            emptyMemory[0x201] = 0xEE;

            emptyMemory[0x208] = 0x00;
            emptyMemory[0x209] = 0xEE;

            emptyMemory[0x30A] = 0x00;
            emptyMemory[0x30B] = 0xEE;

            emptyMemory[0x402] = 0x00;
            emptyMemory[0x403] = 0xEE;

            emptyMemory[0x502] = 0x00;
            emptyMemory[0x503] = 0xEE;
            #endregion

            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");
            Assert.Zero(cpu.SP, "SP Register should be zero");
            CollectionAssert.AreEqual(new ushort[16], cpu.stack, "Stack should be empty");

            var initialStack = new ushort[]
            {
                0x206, 0x308, 0x400, 0x500, 0x000, 0x000, 0x000, 0x000,
                0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000
            };

            initialStack.CopyTo(cpu.stack, 0);

            cpu.SP = 4;

            Assert.AreEqual(4, cpu.SP, "PC Register should be at 4");
            CollectionAssert.AreNotEqual(new uint[16], cpu.stack, "Stack should not be empty");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x502, cpu.PC, "PC Register should be at 0x500");
            Assert.AreEqual(3, cpu.SP, "PC Register should have decremented");
            CollectionAssert.AreEqual(initialStack, cpu.stack, "Stack should stay the same");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x402, cpu.PC, "PC Register should be at 0x400");
            Assert.AreEqual(2, cpu.SP, "PC Register should have decremented");
            CollectionAssert.AreEqual(initialStack, cpu.stack, "Stack should stay the same");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x30A, cpu.PC, "PC Register should be at 0x308");
            Assert.AreEqual(1, cpu.SP, "PC Register should have decremented");
            CollectionAssert.AreEqual(initialStack, cpu.stack, "Stack should stay the same");

            cpu.EmulateCycle();

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x208, cpu.PC, "PC Register should be at 0x206");
            Assert.AreEqual(0, cpu.SP, "PC Register should have decremented");
            CollectionAssert.AreEqual(initialStack, cpu.stack, "Stack should stay the same");

            Assert.Throws<StackOverflowException>(cpu.EmulateCycle, "Should throw a Stack Overflow Exception");

            Assert.AreEqual(0x00EE, cpu.opcode, "Opcode shoud be 0x00EE - RET");
            Assert.AreEqual(0x208, cpu.PC, "PC Register should be at 0x206");
            Assert.AreEqual(0, cpu.SP, "PC Register should have not decremented");
            CollectionAssert.AreEqual(initialStack, cpu.stack, "Stack should stay the same");
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_JP_addr()
        {
            var emptyMemory = new byte[0x1000];
            int index = 0x200;

            for (ushort data = 0x1000; data <= 0x16FF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (ushort counter = 0x200; counter <= 0x5FE; counter += 2)
            {
                cpu.PC = counter;
                int expectedPosition = (cpu.memory[counter] << 8 | cpu.memory[counter + 1]) & 0x0FFF;
                Assert.Throws<InvalidOperationException>(cpu.EmulateCycle, $"Should not jump to position 0x{expectedPosition:X2} [opcode: 0x{cpu.opcode:X4}]");
            }

            for (ushort counter = 0x600; counter <= 0xFFE; counter += 2)
            {
                cpu.PC = counter;
                int expectedPosition = (cpu.memory[counter] << 8 | cpu.memory[counter + 1]) & 0x0FFF;
                cpu.EmulateCycle();
                Assert.AreEqual(expectedPosition, cpu.PC, $"Should jump to position 0x{expectedPosition:X2} instead of 0x{cpu.PC:X2} [opcode: 0x{cpu.opcode:X4}]");
            }

            index = 0x200;

            for (ushort data = 0x1700; data <= 0x1DFF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            cpu.LoadMemory(emptyMemory);

            for (ushort counter = 0x200; counter <= 0xFFE; counter += 2)
            {
                cpu.PC = counter;
                int expectedPosition = (cpu.memory[counter] << 8 | cpu.memory[counter + 1]) & 0x0FFF;
                cpu.EmulateCycle();
                Assert.AreEqual(expectedPosition, cpu.PC, $"Should jump to position 0x{expectedPosition:X2} instead of 0x{cpu.PC:X2} [opcode: 0x{cpu.opcode:X4}]");
            }

            index = 0x200;

            for (ushort data = 0x1E00; data <= 0x1FFF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            cpu.LoadMemory(emptyMemory);

            for (ushort counter = 0x200; counter <= 0x5FE; counter += 2)
            {
                cpu.PC = counter;
                int expectedPosition = (cpu.memory[counter] << 8 | cpu.memory[counter + 1]) & 0x0FFF;
                cpu.EmulateCycle();
                Assert.AreEqual(expectedPosition, cpu.PC, $"Should jump to position 0x{expectedPosition:X2} instead of 0x{cpu.PC:X2} [opcode: 0x{cpu.opcode:X4}]");
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_CALL_addr()
        {
            var emptyMemory = new byte[0x1000];
            #region data initialization
            emptyMemory[0x200] = 0x22;
            emptyMemory[0x201] = 0x06;

            emptyMemory[0x206] = 0x23;
            emptyMemory[0x207] = 0x08;

            emptyMemory[0x308] = 0x24;
            emptyMemory[0x309] = 0x00;

            emptyMemory[0x400] = 0x25;
            emptyMemory[0x401] = 0x00;

            emptyMemory[0x500] = 0x22;
            emptyMemory[0x501] = 0x00;
            #endregion

            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");
            Assert.Zero(cpu.SP, "SP Register should be zero");
            CollectionAssert.AreEqual(new ushort[16], cpu.stack, "Stack should be empty");

            var initialStack = new ushort[]
            {
                0x200, 0x206, 0x308, 0x400, 0x500, 0x200, 0x206, 0x308,
                0x400, 0x500, 0x200, 0x206, 0x308, 0x400, 0x500, 0x200
            };

            cpu.EmulateCycle();

            Assert.AreEqual(0x2206, cpu.opcode, "Opcode shoud be 0x2206 - CALL 0x206");
            Assert.AreEqual(0x206, cpu.PC, "PC Register should be at 0x206");
            Assert.AreEqual(1, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x200, cpu.stack[cpu.SP - 1], "Current stack should point to 0x200");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2308, cpu.opcode, "Opcode shoud be 0x2308 - CALL 0x308");
            Assert.AreEqual(0x308, cpu.PC, "PC Register should be at 0x308");
            Assert.AreEqual(2, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x206, cpu.stack[cpu.SP - 1], "Current stack should point to 0x206");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2400, cpu.opcode, "Opcode shoud be 0x2308 - CALL 0x400");
            Assert.AreEqual(0x400, cpu.PC, "PC Register should be at 0x400");
            Assert.AreEqual(3, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x308, cpu.stack[cpu.SP - 1], "Current stack should point to 0x308");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2500, cpu.opcode, "Opcode shoud be 0x2500 - CALL 0x500");
            Assert.AreEqual(0x500, cpu.PC, "PC Register should be at 0x500");
            Assert.AreEqual(4, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x400, cpu.stack[cpu.SP - 1], "Current stack should point to 0x400");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2200, cpu.opcode, "Opcode shoud be 0x2200 - CALL 0x200");
            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");
            Assert.AreEqual(5, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x500, cpu.stack[cpu.SP - 1], "Current stack should point to 0x500");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2206, cpu.opcode, "Opcode shoud be 0x2206 - CALL 0x206");
            Assert.AreEqual(0x206, cpu.PC, "PC Register should be at 0x206");
            Assert.AreEqual(6, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x200, cpu.stack[cpu.SP - 1], "Current stack should point to 0x200");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2308, cpu.opcode, "Opcode shoud be 0x2308 - CALL 0x308");
            Assert.AreEqual(0x308, cpu.PC, "PC Register should be at 0x308");
            Assert.AreEqual(7, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x206, cpu.stack[cpu.SP - 1], "Current stack should point to 0x206");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2400, cpu.opcode, "Opcode shoud be 0x2308 - JP 0x400");
            Assert.AreEqual(0x400, cpu.PC, "PC Register should be at 0x400");
            Assert.AreEqual(8, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x308, cpu.stack[cpu.SP - 1], "Current stack should point to 0x308");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2500, cpu.opcode, "Opcode shoud be 0x2500 - CALL 0x500");
            Assert.AreEqual(0x500, cpu.PC, "PC Register should be at 0x500");
            Assert.AreEqual(9, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x400, cpu.stack[cpu.SP - 1], "Current stack should point to 0x400");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2200, cpu.opcode, "Opcode shoud be 0x2200 - CALL 0x200");
            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");
            Assert.AreEqual(10, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x500, cpu.stack[cpu.SP - 1], "Current stack should point to 0x500");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2206, cpu.opcode, "Opcode shoud be 0x2206 - CALL 0x206");
            Assert.AreEqual(0x206, cpu.PC, "PC Register should be at 0x206");
            Assert.AreEqual(11, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x200, cpu.stack[cpu.SP - 1], "Current stack should point to 0x200");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2308, cpu.opcode, "Opcode shoud be 0x2308 - CALL 0x308");
            Assert.AreEqual(0x308, cpu.PC, "PC Register should be at 0x308");
            Assert.AreEqual(12, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x206, cpu.stack[cpu.SP - 1], "Current stack should point to 0x206");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2400, cpu.opcode, "Opcode shoud be 0x2308 - CALL 0x400");
            Assert.AreEqual(0x400, cpu.PC, "PC Register should be at 0x400");
            Assert.AreEqual(13, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x308, cpu.stack[cpu.SP - 1], "Current stack should point to 0x308");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2500, cpu.opcode, "Opcode shoud be 0x2500 - CALL 0x500");
            Assert.AreEqual(0x500, cpu.PC, "PC Register should be at 0x500");
            Assert.AreEqual(14, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x400, cpu.stack[cpu.SP - 1], "Current stack should point to 0x400");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2200, cpu.opcode, "Opcode shoud be 0x2200 - CALL 0x200");
            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");
            Assert.AreEqual(15, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x500, cpu.stack[cpu.SP - 1], "Current stack should point to 0x500");

            cpu.EmulateCycle();

            Assert.AreEqual(0x2206, cpu.opcode, "Opcode shoud be 0x2206 - CALL 0x206");
            Assert.AreEqual(0x206, cpu.PC, "PC Register should be at 0x206");
            Assert.AreEqual(16, cpu.SP, "PC Register should have incremented");
            Assert.AreEqual(0x200, cpu.stack[cpu.SP - 1], "Current stack should point to 0x200");

            Assert.Throws<StackOverflowException>(cpu.EmulateCycle, "Should throw a Stack Overflow Exception");
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SE_Vx_byte()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int register = 0; register < 7; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x30 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_SE_Vx_byte(emptyMemory, testAddress);

            emptyMemory = new byte[0x1000];
            testAddress = 0x200;

            for (int register = 7; register < 14; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x30 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_SE_Vx_byte(emptyMemory, testAddress);

            emptyMemory = new byte[0x1000];
            testAddress = 0x200;

            for (int register = 14; register <= 0xF; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x30 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_SE_Vx_byte(emptyMemory, testAddress);
        }

        private void Test_SE_Vx_byte(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int register = cpu.memory[address] & 0x0F;
                byte value = cpu.memory[address + 1];
                ushort opcode = unchecked((ushort)((0x30 | (register & 0x0F)) << 8 | value));

                cpu.PC = address;

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = unchecked((byte)~value);
                }

                cpu.EmulateCycle();

                Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SE V{register:X1}, 0x{value:X2}");
                Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when V{register:X1} different from 0x{value:X2} (actual value: 0x{cpu.V[register]:X2})");

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.PC = address;
                    cpu.V[index] = value;

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SE V{register:X1}, 0x{value:X2}");

                    if (index == register)
                    {
                        Assert.AreEqual(address + 4, cpu.PC, $"Should skip the next instruction when V{index:X1}=0x{cpu.V[index]:X2}");
                    }
                    else
                    {
                        Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when V{register:X1}=0x{cpu.V[register]:X2} and V{index:X1}=0x{cpu.V[index]:X2}");
                    }

                    cpu.V[index] = unchecked((byte)~value);
                }

                for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                {
                    cpu.PC = address;
                    cpu.V[register] = unchecked((byte)testValue);

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SE V{register:X1}, 0x{value:X2}");

                    if (testValue == value)
                    {
                        Assert.AreEqual(address + 4, cpu.PC, $"Should skip the next instruction when V{register:X1}=0x{testValue:X2}");
                    }
                    else
                    {
                        Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when V{register:X1}=0x{testValue:X2} (should skip only with 0x{value})");
                    }

                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SNE_Vx_byte()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int register = 0; register < 7; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x40 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_SNE_Vx_byte(emptyMemory, testAddress);

            emptyMemory = new byte[0x1000];
            testAddress = 0x200;

            for (int register = 7; register < 14; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x40 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_SNE_Vx_byte(emptyMemory, testAddress);

            emptyMemory = new byte[0x1000];
            testAddress = 0x200;

            for (int register = 14; register <= 0xF; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x40 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_SNE_Vx_byte(emptyMemory, testAddress);
        }

        private void Test_SNE_Vx_byte(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int register = cpu.memory[address] & 0x0F;
                byte value = cpu.memory[address + 1];
                ushort opcode = unchecked((ushort)((0x40 | (register & 0x0F)) << 8 | value));

                cpu.PC = address;

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = value;
                }

                cpu.EmulateCycle();

                Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SNE V{register:X1}, 0x{value:X2}");
                Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when V{register:X1} is 0x{value:X2} (actual value: 0x{cpu.V[register]:X2}");

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.PC = address;
                    cpu.V[index] = unchecked((byte)~value);

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SNE V{register:X1}, 0x{value:X2}");

                    if (index == register)
                    {
                        Assert.AreEqual(address + 4, cpu.PC, $"Should skip the next instruction when when V{register:X1}=0x{cpu.V[register]:X2}");
                    }
                    else
                    {
                        Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction V{index:X1}=0x{cpu.V[index]:X2} and V{index:X1}=0x{cpu.V[index]:X2}");
                    }

                    cpu.V[index] = value;
                }

                for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                {
                    cpu.PC = address;
                    cpu.V[register] = unchecked((byte)testValue);

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SNE V{register:X1}, 0x{value:X2}");

                    if (testValue == value)
                    {
                        Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when V{register:X1}=0x{testValue:X2}");
                    }
                    else
                    {
                        Assert.AreEqual(address + 4, cpu.PC, $"Should skip the next instruction when V{register:X1}=0x{testValue:X2} (should skip only with 0x{value})");
                    }

                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SE_Vx_Vy()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX:X1} : V{registerY:X1}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x50 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)((registerY & 0x0F) << 8));
                }
            }

            Test_SE_Vx_Vy(emptyMemory, testAddress);
        }

        private void Test_SE_Vx_Vy(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                byte value = unchecked((byte)rnd.Next(0x00, 0x100));

                ushort opcode = unchecked((ushort)((0x50 | (registerX & 0x0F)) << 8 | (registerY << 8)));

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = value;
                }

                if (registerX != registerY)
                {
                    cpu.PC = address;

                    cpu.V[registerX] = unchecked((byte)~value);

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SE V{registerX:X1}, V{registerY:X1}");
                    Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when V{registerX:X1} ({cpu.V[registerX]:X2}) is not the same value as V{registerY:X1} ({cpu.V[registerY]:X2})");
                }

                cpu.PC = address;

                cpu.V[registerX] = value;

                cpu.EmulateCycle();

                Assert.AreEqual(address + 4, cpu.PC, $"Should skip the next instruction when V{registerX:X1} ({cpu.V[registerX]:X2}) is the same value as V{registerY:X1} ({cpu.V[registerY]:X2})");

                for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                {
                    cpu.PC = address;
                    cpu.V[registerX] = unchecked((byte)testValue);
                    cpu.V[registerY] = unchecked((byte)~testValue);

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SE V{registerX:X1}, V{registerY:X1}");

                    if (registerX == registerY)
                    {
                        Assert.AreEqual(address + 4, cpu.PC, $"Should skip the next instruction when comparing V{registerX:X1} to itself");
                    }
                    else
                    {
                        Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when V{registerX:X1} ({cpu.V[registerX]:X2}) is not the same value as V{registerY:X1} ({cpu.V[registerY]:X2})");
                    }

                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_Vx_byte()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int register = 0; register < 7; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x60 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_LD_Vx_byte(emptyMemory, testAddress);

            emptyMemory = new byte[0x1000];
            testAddress = 0x200;

            for (int register = 7; register < 14; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x60 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_LD_Vx_byte(emptyMemory, testAddress);

            emptyMemory = new byte[0x1000];
            testAddress = 0x200;

            for (int register = 14; register <= 0xF; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x60 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_LD_Vx_byte(emptyMemory, testAddress);
        }

        private void Test_LD_Vx_byte(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int register = cpu.memory[address] & 0x0F;
                byte value = cpu.memory[address + 1];
                byte invertedValue = unchecked((byte)~value);
                ushort opcode = unchecked((ushort)((0x60 | (register & 0x0F)) << 8 | value));

                cpu.PC = address;

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = invertedValue;
                }

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.PC = address;
                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - LD V{register:X1}, 0x{value:X2}");

                    if (index == register)
                    {
                        Assert.AreEqual(value, cpu.V[index], $"V{index:X1} should be 0x{value:X2}");
                    }
                    else
                    {
                        Assert.AreNotEqual(value, cpu.V[index], $"V{index:X1} should not be 0x{value:X2}");
                        Assert.AreEqual(invertedValue, cpu.V[index], $"V{index:X1} should be 0x{invertedValue:X2}");
                    }

                    cpu.V[index] = invertedValue;
                    cpu.V[register] = invertedValue;
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_ADD_Vx_byte()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int register = 0; register < 7; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x70 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_ADD_Vx_byte(emptyMemory, testAddress);

            emptyMemory = new byte[0x1000];
            testAddress = 0x200;

            for (int register = 7; register < 14; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x70 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_ADD_Vx_byte(emptyMemory, testAddress);

            emptyMemory = new byte[0x1000];
            testAddress = 0x200;

            for (int register = 14; register <= 0xF; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{register}: {value}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x70 | (register & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)value);
                }
            }

            Test_ADD_Vx_byte(emptyMemory, testAddress);
        }

        private void Test_ADD_Vx_byte(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int register = cpu.memory[address] & 0x0F;
                byte value = cpu.memory[address + 1];
                byte invertedValue = unchecked((byte)~value);
                ushort opcode = unchecked((ushort)((0x70 | (register & 0x0F)) << 8 | value));

                cpu.PC = address;

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = invertedValue;
                }

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.PC = address;
                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - ADD V{register:X1}, 0x{value:X2}");

                    if (index == register)
                    {
                        Assert.AreEqual(0xFF, cpu.V[index], $"V{index:X1} should be 0xFF (0x{invertedValue:X2} + 0x{value:X2}) [opcode: 0x{opcode:X4}] [V{index:X1}= 0x{cpu.V[index]:X2}]");
                    }
                    else
                    {
                        Assert.AreNotEqual(value, cpu.V[index], $"V{index:X1} should not be 0x{value:X2}");
                        Assert.AreEqual(invertedValue, cpu.V[index], $"V{index:X1} should be 0x{invertedValue:X2}");
                    }

                    cpu.V[index] = invertedValue;
                    cpu.V[register] = invertedValue;
                }

                for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                {
                    cpu.PC = address;
                    cpu.V[register] = unchecked((byte)testValue);

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - ADD V{register:X1}, 0x{value:X2}");
                    byte sum = unchecked((byte)(testValue + value));
                    Assert.AreEqual(sum, cpu.V[register], $"V{register:X1} should be {sum} (0x{testValue:X2} + 0x{value:X2}) [opcode: 0x{opcode:X4}] [V{register:X1}= 0x{cpu.V[register]:X2}]");
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SNE_Vx_Vy()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX:X1} : V{registerY:X1}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x90 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)((registerY & 0x0F) << 8));
                }
            }

            Test_SNE_Vx_Vy(emptyMemory, testAddress);
        }

        private void Test_SNE_Vx_Vy(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                byte value = unchecked((byte)rnd.Next(0x00, 0x100));

                ushort opcode = unchecked((ushort)((0x90 | (registerX & 0x0F)) << 8 | (registerY << 8)));

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = value;
                }

                if (registerX != registerY)
                {
                    cpu.PC = address;

                    cpu.V[registerX] = unchecked((byte)~value);

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SNE V{registerX:X1}, V{registerY:X1}");
                    Assert.AreEqual(address + 4, cpu.PC, $"Should skip the next instruction when V{registerX:X1} ({cpu.V[registerX]:X2}) is not the same value as V{registerY:X1} ({cpu.V[registerY]:X2})");
                }

                cpu.PC = address;

                cpu.V[registerX] = value;

                cpu.EmulateCycle();

                Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when V{registerX:X1} ({cpu.V[registerX]:X2}) is the same value as V{registerY:X1} ({cpu.V[registerY]:X2})");

                for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                {
                    cpu.PC = address;
                    cpu.V[registerX] = unchecked((byte)testValue);
                    cpu.V[registerY] = unchecked((byte)~testValue);

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SNE V{registerX:X1}, V{registerY:X1}");

                    if (registerX == registerY)
                    {
                        Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when comparing V{registerX:X1} to itself");
                    }
                    else
                    {
                        Assert.AreEqual(address + 4, cpu.PC, $"Should skip the next instruction when V{registerX:X1} ({cpu.V[registerX]:X2}) is not the same value as V{registerY:X1} ({cpu.V[registerY]:X2})");
                    }

                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_ADD_Vx_Vy()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX}: V{registerY}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x80 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)(((registerY & 0x0F) << 4) | 0x04));
                }
            }

            Test_ADD_Vx_Vy(emptyMemory, testAddress);
        }

        private void Test_ADD_Vx_Vy(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                ushort opcode = unchecked((ushort)((0x80 | (registerX & 0x0F)) << 8 | ((registerY & 0x0F) << 4) | 0x04));

                for (int testValueX = 0x00; testValueX <= 0xFF; testValueX++)
                {
                    for (int testValueY = 0x00; testValueY <= 0xFF; testValueY++)
                    {
                        int sum;
                        cpu.PC = address;
                        cpu.V[registerX] = unchecked((byte)testValueX);

                        if (registerX != registerY)
                        {
                            cpu.V[registerY] = unchecked((byte)testValueY);
                            sum = testValueX + testValueY;
                        }
                        else
                        {
                            sum = testValueX + testValueX;
                        }

                        byte value = unchecked((byte)sum);
                        byte carryFlag = unchecked((byte)(sum >= 0x100 ? 1 : 0));

                        if (registerX >= 0xF || registerY >= 0xF)
                        {
                            Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                        }
                        else
                        {
                            cpu.EmulateCycle();
                        }

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - ADD V{registerX:X1}, V{registerY:X1}");

                        if (registerX != 0xF && registerY != 0xF)
                        {
                            Assert.AreEqual(value, cpu.V[registerX], $"V{registerX:X1} should be {sum} (0x{testValueX:X2} + 0x{testValueY:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                            Assert.AreEqual(carryFlag, cpu.V[0xF], $"VF should be {carryFlag} (0x{testValueX:X2} + 0x{testValueY:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                        }
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_OR_Vx_Vy()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX}: V{registerY}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x80 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)(((registerY & 0x0F) << 4) | 0x01));
                }
            }

            Test_OR_Vx_Vy(emptyMemory, testAddress);
        }

        private void Test_OR_Vx_Vy(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                ushort opcode = unchecked((ushort)((0x80 | (registerX & 0x0F)) << 8 | ((registerY & 0x0F) << 4) | 0x01));

                for (int testValueX = 0x00; testValueX <= 0xFF; testValueX++)
                {
                    for (int testValueY = 0x00; testValueY <= 0xFF; testValueY++)
                    {
                        byte value;
                        cpu.PC = address;
                        cpu.V[registerX] = unchecked((byte)testValueX);

                        if (registerX != registerY)
                        {
                            cpu.V[registerY] = unchecked((byte)testValueY);
                            value = unchecked((byte)(testValueX | testValueY));
                        }
                        else
                        {
                            value = unchecked((byte)(testValueX | testValueX));
                        }

                        cpu.EmulateCycle();

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - OR V{registerX:X1}, V{registerY:X1}");
                        Assert.AreEqual(value, cpu.V[registerX], $"V{registerX:X1} should be {value} (0x{testValueX:X2} OR 0x{testValueY:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_AND_Vx_Vy()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX}: V{registerY}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x80 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)(((registerY & 0x0F) << 4) | 0x02));
                }
            }

            Test_AND_Vx_Vy(emptyMemory, testAddress);
        }

        private void Test_AND_Vx_Vy(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                ushort opcode = unchecked((ushort)((0x80 | (registerX & 0x0F)) << 8 | ((registerY & 0x0F) << 4) | 0x02));

                for (int testValueX = 0x00; testValueX <= 0xFF; testValueX++)
                {
                    for (int testValueY = 0x00; testValueY <= 0xFF; testValueY++)
                    {
                        byte value;
                        cpu.PC = address;
                        cpu.V[registerX] = unchecked((byte)testValueX);

                        if (registerX != registerY)
                        {
                            cpu.V[registerY] = unchecked((byte)testValueY);
                            value = unchecked((byte)(testValueX & testValueY));
                        }
                        else
                        {
                            value = unchecked((byte)(testValueX & testValueX));
                        }

                        cpu.EmulateCycle();

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - AND V{registerX:X1}, V{registerY:X1}");
                        Assert.AreEqual(value, cpu.V[registerX], $"V{registerX:X1} should be {value} (0x{testValueX:X2} AND 0x{testValueY:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_XOR_Vx_Vy()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX}: V{registerY}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x80 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)(((registerY & 0x0F) << 4) | 0x03));
                }
            }

            Test_XOR_Vx_Vy(emptyMemory, testAddress);
        }

        private void Test_XOR_Vx_Vy(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                ushort opcode = unchecked((ushort)((0x80 | (registerX & 0x0F)) << 8 | ((registerY & 0x0F) << 4) | 0x03));

                for (int testValueX = 0x00; testValueX <= 0xFF; testValueX++)
                {
                    for (int testValueY = 0x00; testValueY <= 0xFF; testValueY++)
                    {
                        byte value;
                        cpu.PC = address;
                        cpu.V[registerX] = unchecked((byte)testValueX);

                        if (registerX != registerY)
                        {
                            cpu.V[registerY] = unchecked((byte)testValueY);
                            value = unchecked((byte)(testValueX ^ testValueY));
                        }
                        else
                        {
                            value = unchecked((byte)(testValueX ^ testValueX));
                        }

                        cpu.EmulateCycle();

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - XOR V{registerX:X1}, V{registerY:X1}");
                        Assert.AreEqual(value, cpu.V[registerX], $"V{registerX:X1} should be {value} (0x{testValueX:X2} XOR 0x{value:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SUB_Vx_Vy()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX}: V{registerY}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x80 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)(((registerY & 0x0F) << 4) | 0x05));
                }
            }

            Test_SUB_Vx_Vy(emptyMemory, testAddress);
        }

        private void Test_SUB_Vx_Vy(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                ushort opcode = unchecked((ushort)((0x80 | (registerX & 0x0F)) << 8 | ((registerY & 0x0F) << 4) | 0x05));

                for (int testValueX = 0x00; testValueX <= 0xFF; testValueX++)
                {
                    for (int testValueY = 0x00; testValueY <= 0xFF; testValueY++)
                    {
                        int sub;
                        cpu.PC = address;
                        cpu.V[registerX] = unchecked((byte)testValueX);

                        if (registerX != registerY)
                        {
                            cpu.V[registerY] = unchecked((byte)testValueY);
                            sub = testValueX - testValueY;
                        }
                        else
                        {
                            sub = testValueX - testValueX;
                        }

                        byte value = unchecked((byte)sub);
                        byte carryFlag = unchecked((byte)(sub > 0 ? 1 : 0));

                        if (registerX >= 0xF || registerY >= 0xF)
                        {
                            Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                        }
                        else
                        {
                            cpu.EmulateCycle();
                        }

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SUB V{registerX:X1}, V{registerY:X1}");

                        if (registerX != 0xF && registerY != 0xF)
                        {
                            Assert.AreEqual(value, cpu.V[registerX], $"V{registerX:X1} should be {sub} (0x{testValueX:X2} - 0x{testValueY:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                            Assert.AreEqual(carryFlag, cpu.V[0xF], $"VF should be {carryFlag} (0x{testValueX:X2} - 0x{testValueY:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                        }
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_Vx_Vy()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX:X1} : V{registerY:X1}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x80 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)((registerY & 0x0F) << 4));
                }
            }

            Test_LD_Vx_Vy(emptyMemory, testAddress);
        }

        private void Test_LD_Vx_Vy(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                byte value = unchecked((byte)rnd.Next(0x00, 0x100));

                ushort opcode = unchecked((ushort)((0x80 | (registerX & 0x0F)) << 8 | ((registerY & 0x0F) << 4)));

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = value;
                }

                for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                {
                    cpu.PC = address;
                    byte invertedValue = unchecked((byte)~testValue);
                    cpu.V[registerX] = unchecked((byte)testValue);

                    if (registerX == registerY)
                    {
                        if (testValue == value)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (invertedValue == value)
                        {
                            continue;
                        }

                        cpu.V[registerY] = invertedValue;
                    }

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - LD V{registerX:X1}, V{registerY:X1}");

                    if (registerX == registerY)
                    {
                        Assert.AreNotEqual(value, cpu.V[registerX], $"V{registerX:X1} should not be 0x{value:X2}");
                        Assert.AreEqual(testValue, cpu.V[registerX], $"V{registerX:X1} should be 0x{testValue:X2}");
                    }
                    else
                    {
                        Assert.AreNotEqual(value, cpu.V[registerX], $"V{registerX:X1} should not be 0x{value:X2}");
                        Assert.AreEqual(invertedValue, cpu.V[registerX], $"V{registerX:X1} should be 0x{invertedValue:X2}");
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SHR_Vx()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX}: V{registerY}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x80 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)(((registerY & 0x0F) << 4) | 0x06));
                }
            }

            Test_SHR_Vx(emptyMemory, testAddress);
        }

        private void Test_SHR_Vx(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                ushort opcode = unchecked((ushort)((0x80 | (registerX & 0x0F)) << 8 | ((registerY & 0x0F) << 4) | 0x06));

                for (int testValueX = 0x00; testValueX <= 0xFF; testValueX++)
                {
                    for (int testValueY = 0x00; testValueY <= 0xFF; testValueY++)
                    {
                        cpu.PC = address;
                        cpu.V[registerX] = unchecked((byte)testValueX);

                        if (registerX != registerY)
                        {
                            cpu.V[registerY] = unchecked((byte)testValueY);
                        }

                        byte value = unchecked((byte)(testValueX >> 1));
                        byte carryFlag = unchecked((byte)(testValueX & 0x01));

                        if (registerX >= 0xF || registerY >= 0xF)
                        {
                            Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                        }
                        else
                        {
                            cpu.EmulateCycle();
                        }

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SHR V{registerX:X1}, {{V{registerY:X1}}}");

                        if (registerX != 0xF && registerY != 0xF)
                        {
                            Assert.AreEqual(value, cpu.V[registerX], $"V{registerX:X1} should be {value} (0x{testValueX:X2} >> 1) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                            Assert.AreEqual(carryFlag, cpu.V[0xF], $"VF should be {carryFlag} (0x{testValueX:X2} >> 1) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                        }
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SUBN_Vx_Vy()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX}: V{registerY}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x80 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)(((registerY & 0x0F) << 4) | 0x07));
                }
            }

            Test_SUBN_Vx_Vy(emptyMemory, testAddress);
        }

        private void Test_SUBN_Vx_Vy(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                ushort opcode = unchecked((ushort)((0x80 | (registerX & 0x0F)) << 8 | ((registerY & 0x0F) << 4) | 0x07));

                for (int testValueX = 0x00; testValueX <= 0xFF; testValueX++)
                {
                    for (int testValueY = 0x00; testValueY <= 0xFF; testValueY++)
                    {
                        int sub;
                        cpu.PC = address;
                        cpu.V[registerX] = unchecked((byte)testValueX);

                        if (registerX != registerY)
                        {
                            cpu.V[registerY] = unchecked((byte)testValueY);
                            sub = testValueY - testValueX;
                        }
                        else
                        {
                            sub = testValueX - testValueX;
                        }

                        byte value = unchecked((byte)sub);
                        byte carryFlag = unchecked((byte)(sub > 0 ? 1 : 0));

                        if (registerX >= 0xF || registerY >= 0xF)
                        {
                            Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                        }
                        else
                        {
                            cpu.EmulateCycle();
                        }

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SUBN V{registerX:X1}, V{registerY:X1}");

                        if (registerX != 0xF && registerY != 0xF)
                        {
                            Assert.AreEqual(value, cpu.V[registerX], $"V{registerX:X1} should be {sub} (0x{testValueX:X2} - 0x{testValueY:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                            Assert.AreEqual(carryFlag, cpu.V[0xF], $"VF should be {carryFlag} (0x{testValueX:X2} - 0x{testValueY:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                        }
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SHL_Vx()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                for (int registerY = 0; registerY <= 0xF; registerY++, testAddress += 2)
                {
                    if (testAddress >= 0x1000)
                    {
                        Assert.Fail($"[{testAddress}]: V{registerX}: V{registerY}");
                        break;
                    }

                    emptyMemory[testAddress] = unchecked((byte)(0x80 | (registerX & 0x0F)));
                    emptyMemory[testAddress + 1] = unchecked((byte)(((registerY & 0x0F) << 4) | 0x0E));
                }
            }

            Test_SHL_Vx(emptyMemory, testAddress);
        }

        private void Test_SHL_Vx(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                int registerY = (cpu.memory[address + 1] & 0xF0) >> 4;
                ushort opcode = unchecked((ushort)((0x80 | (registerX & 0x0F)) << 8 | ((registerY & 0x0F) << 4) | 0x0E));

                for (int testValueX = 0x00; testValueX <= 0xFF; testValueX++)
                {
                    for (int testValueY = 0x00; testValueY <= 0xFF; testValueY++)
                    {
                        cpu.PC = address;
                        cpu.V[registerX] = unchecked((byte)testValueX);

                        if (registerX != registerY)
                        {
                            cpu.V[registerY] = unchecked((byte)testValueY);
                        }

                        byte value = unchecked((byte)(testValueX << 1));
                        byte carryFlag = unchecked((byte)((testValueX & 0x80) >> 7));

                        if (registerX >= 0xF || registerY >= 0xF)
                        {
                            Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                        }
                        else
                        {
                            cpu.EmulateCycle();
                        }

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SHL V{registerX:X1}, {{V{registerY:X1}}}");

                        if (registerX != 0xF && registerY != 0xF)
                        {
                            Assert.AreEqual(value, cpu.V[registerX], $"V{registerX:X1} should be {value} (0x{testValueX:X2} << 1) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                            Assert.AreEqual(carryFlag, cpu.V[0xF], $"VF should be {carryFlag} (0x{testValueX:X2} >> 1) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}] [V{registerY:X1}= 0x{cpu.V[registerY]:X2}]");
                        }
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_I_addr()
        {
            var emptyMemory = new byte[0x1000];
            int index = 0x200;

            for (ushort data = 0xA000; data <= 0xA6FF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (ushort counter = 0x200; counter <= 0xFFE; counter += 2)
            {
                cpu.PC = counter;
                int expectedPosition = (cpu.memory[counter] << 8 | cpu.memory[counter + 1]) & 0x0FFF;
                cpu.EmulateCycle();
                Assert.AreEqual(expectedPosition, cpu.I, $"Register I should point to position 0x{expectedPosition:X2} instead of 0x{cpu.PC:X2} [opcode: 0x{cpu.opcode:X4}]");
            }

            index = 0x200;

            for (ushort data = 0xA700; data <= 0xADFF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            cpu.LoadMemory(emptyMemory);

            for (ushort counter = 0x200; counter <= 0xFFE; counter += 2)
            {
                cpu.PC = counter;
                int expectedPosition = (cpu.memory[counter] << 8 | cpu.memory[counter + 1]) & 0x0FFF;
                cpu.EmulateCycle();
                Assert.AreEqual(expectedPosition, cpu.I, $"Register I should point to position 0x{expectedPosition:X2} instead of 0x{cpu.PC:X2} [opcode: 0x{cpu.opcode:X4}]");
            }

            index = 0x200;

            for (ushort data = 0xAE00; data <= 0xAFFF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            cpu.LoadMemory(emptyMemory);

            for (ushort counter = 0x200; counter <= 0x5FE; counter += 2)
            {
                cpu.PC = counter;
                int expectedPosition = (cpu.memory[counter] << 8 | cpu.memory[counter + 1]) & 0x0FFF;
                cpu.EmulateCycle();
                Assert.AreEqual(expectedPosition, cpu.I, $"Register I should point to position 0x{expectedPosition:X2} instead of 0x{cpu.PC:X2} [opcode: 0x{cpu.opcode:X4}]");
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_JP_V0_addr()
        {
            var emptyMemory = new byte[0x1000];
            int index = 0x200;

            for (ushort data = 0xB000; data <= 0xB6FF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (ushort counter = 0x200; counter <= 0xFFE; counter += 2)
            {
                int nnn = (cpu.memory[counter] << 8 | cpu.memory[counter + 1]) & 0x0FFF;

                for (int value = 0x00; value <= 0xFF; value++)
                {
                    cpu.PC = counter;
                    cpu.V[0] = unchecked((byte)value);

                    int dest = nnn + value;

                    if (dest < 0x200 || dest > 0xFFF)
                    {
                        Assert.Throws<InvalidOperationException>(cpu.EmulateCycle, $"Should not jump to position 0x{(ushort)dest:X3} [opcode: 0x{cpu.opcode:X4}] (V[0]={cpu.V[0]:X2})");
                    }
                    else
                    {
                        cpu.EmulateCycle();
                        Assert.AreEqual((ushort)dest, cpu.PC, $"Should jump to position 0x{(ushort)dest:X3} instead of 0x{cpu.PC:X2} [opcode: 0x{cpu.opcode:X4}] (V[0]={cpu.V[0]:X2})");
                    }
                }
            }

            index = 0x200;

            for (ushort data = 0xB700; data <= 0xBDFF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            cpu.LoadMemory(emptyMemory);

            for (ushort counter = 0x200; counter <= 0xFFE; counter += 2)
            {
                int nnn = (cpu.memory[counter] << 8 | cpu.memory[counter + 1]) & 0x0FFF;

                for (int value = 0x00; value <= 0xFF; value++)
                {
                    cpu.PC = counter;
                    cpu.V[0] = unchecked((byte)value);

                    int dest = nnn + value;

                    if (dest < 0x200 || dest > 0xFFF)
                    {
                        Assert.Throws<InvalidOperationException>(cpu.EmulateCycle, $"Should not jump to position 0x{(ushort)dest:X3} [opcode: 0x{cpu.opcode:X4}] (V[0]={cpu.V[0]:X2})");
                    }
                    else
                    {
                        cpu.EmulateCycle();
                        Assert.AreEqual((ushort)dest, cpu.PC, $"Should jump to position 0x{(ushort)dest:X3} instead of 0x{cpu.PC:X2} [opcode: 0x{cpu.opcode:X4}] (V[0]={cpu.V[0]:X2})");
                    }
                }
            }

            index = 0x200;

            for (ushort data = 0xBE00; data <= 0xBFFF; data++)
            {
                byte high = unchecked((byte)((data & 0xFF00) >> 8));
                byte low = unchecked((byte)(data & 0x00FF));

                emptyMemory[index] = high;
                index++;
                emptyMemory[index] = low;
                index++;
            }

            cpu.LoadMemory(emptyMemory);

            for (ushort counter = 0x200; counter <= 0x5FE; counter += 2)
            {
                int nnn = (cpu.memory[counter] << 8 | cpu.memory[counter + 1]) & 0x0FFF;

                for (int value = 0x00; value <= 0xFF; value++)
                {
                    cpu.PC = counter;
                    cpu.V[0] = unchecked((byte)value);

                    int dest = nnn + value;

                    if (dest < 0x200 || dest > 0xFFF)
                    {
                        Assert.Throws<InvalidOperationException>(cpu.EmulateCycle, $"Should not jump to position 0x{(ushort)dest:X3} [opcode: 0x{cpu.opcode:X4}] (V[0]={cpu.V[0]:X2})");
                    }
                    else
                    {
                        cpu.EmulateCycle();
                        Assert.AreEqual((ushort)dest, cpu.PC, $"Should jump to position 0x{(ushort)dest:X3} instead of 0x{cpu.PC:X2} [opcode: 0x{cpu.opcode:X4}] (V[0]={cpu.V[0]:X2})");
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_RND_Vx_byte()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int register = 0; register <= 0xF; register++)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xC0 | (register & 0x0F)));
                emptyMemory[testAddress + 1] = 0xFF;
            }

            Test_RND_Vx_0xFF(emptyMemory, testAddress);

            emptyMemory = new byte[0x1000];
            testAddress = 0x200;

            for (int value = 0x00; value <= 0xFF; value++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V0: {value}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xC0 | (0x00 & 0x0F)));
                emptyMemory[testAddress + 1] = unchecked((byte)value);
            }

            Test_RND_V0_byte(emptyMemory, testAddress);
        }

        private void Test_RND_Vx_0xFF(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int register = cpu.memory[address] & 0x0F;
                byte value = cpu.memory[address + 1];
                byte invertedValue = unchecked((byte)~value);
                ushort opcode = unchecked((ushort)((0xC0 | (register & 0x0F)) << 8 | value));

                cpu.PC = address;

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = invertedValue;
                }

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.PC = address;
                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - RND V{register:X1}, 0x{value:X2}");

                    if (index == register)
                    {
                        Assert.AreNotEqual(invertedValue, cpu.V[index], $"V{index:X1} should not be 0x{invertedValue:X2} [opcode: 0x{opcode:X4}] [V{index:X1}= 0x{cpu.V[index]:X2}]");
                    }
                    else
                    {
                        Assert.AreEqual(invertedValue, cpu.V[index], $"V{index:X1} should be 0x{invertedValue:X2}");
                    }

                    cpu.V[index] = invertedValue;
                    cpu.V[register] = invertedValue;
                }

                const int COUNT = 4096;

                var randomNums = new List<uint>(COUNT);
                Assert.IsFalse(IsRandom(randomNums.ToArray(), 0xFF), $"RND V{register:X1}, 0x{value:X2} seems to be random: {{string.Join(", ", randomNums)}}");

                for (int counter = 0; counter < COUNT; counter++)
                {
                    cpu.PC = address;
                    cpu.EmulateCycle();
                    randomNums.Add(cpu.V[register]);
                }

                uint[] randomNumArray = randomNums.ToArray();
                Assert.IsTrue(IsRandom(randomNumArray, 0xFF));

                var bitArray = MakeBitArray(randomNumArray);

                double pFreq = FrequencyTest(bitArray);
                Assert.GreaterOrEqual(pFreq, 0.01, $"There is evidence that sequence is NOT random for Frequency test = {pFreq:F4}");

                int blockLength = 8;
                double pBlock = BlockTest(bitArray, blockLength);
                Assert.GreaterOrEqual(pBlock, 0.01, $"There is evidence that sequence is NOT random for Block test = {pBlock:F4}");

                double pRuns = RunsTest(bitArray);
                Assert.GreaterOrEqual(pRuns, 0.01, $"There is evidence that sequence is NOT random for Runs test = {pRuns:F4}");

                for (int number = 0x00; number <= 0xFF; number++)
                {
                    Assert.IsTrue(randomNums.Any(n => n == number), $"RND V{register:X1}, 0x{value:X2} cannot find any value 0x{number:X2}: {{string.Join(", " , randomNums)}}");
                }
            }
        }

        private void Test_RND_V0_byte(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int register = cpu.memory[address] & 0x0F;
                byte value = cpu.memory[address + 1];
                byte invertedValue = unchecked((byte)~value);
                ushort opcode = unchecked((ushort)((0xC0 | (register & 0x0F)) << 8 | value));

                cpu.PC = address;

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = invertedValue;
                }

                const int COUNT = 4096;

                var randomNums = new List<uint>(COUNT);
                Assert.IsFalse(IsRandom(randomNums.ToArray(), 0xFF));

                for (int counter = 0; counter < COUNT; counter++)
                {
                    cpu.PC = address;
                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - RND V{register:X1}, 0x{value:X2}");

                    for (int index = 0; index <= 0xF; index++)
                    {
                        if (index == register)
                        {
                            continue;
                        }

                        Assert.AreEqual(invertedValue, cpu.V[index], $"V{index:X1} should be 0x{value:X2}");
                    }

                    Assert.Zero(cpu.V[register] & invertedValue, $"V{register:X1} AND 0x{invertedValue:X2} should be 0x00");

                    randomNums.Add(cpu.V[register]);

                    cpu.V[register] = invertedValue;
                }

                /*
                if (value > 0x00)
                {
                    var bitArray = MakeBitArray(randomNums.ToArray());

                    double pFreq = FrequencyTest(bitArray);
                    Assert.GreaterOrEqual(pFreq, 0.01, $"There is evidence that sequence is NOT random for Frequency test = {pFreq:F4} RND V{register:X1}, 0x{value:X2}: {string.Join(", ", randomNums)}");

                    int blockLength = 8;
                    double pBlock = BlockTest(bitArray, blockLength);
                    Assert.GreaterOrEqual(pBlock, 0.01, $"There is evidence that sequence is NOT random for Block test = {pBlock:F4} RND V{register:X1}, 0x{value:X2}: {string.Join(", ", randomNums)}");

                    double pRuns = RunsTest(bitArray);
                    Assert.GreaterOrEqual(pRuns, 0.01, $"There is evidence that sequence is NOT random for Runs test = {pRuns:F4} RND V{register:X1}, 0x{value:X2}: {string.Join(", ", randomNums)}");
                }
                */

                for (int number = 0x00; number <= 0xFF; number++)
                {
                    Assert.IsTrue(randomNums.Any(n => n == (number & value)), $"RND V{register:X1}, 0x{value:X2} cannot find any value 0x{(number & value):X2}: {{string.Join(", " , randomNums)}}");
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Support Tests")]
        public void MakeBitArrayTest()
        {
            const string bitString1 = "1100 1001 0000 1111 1101 1010 1010 0010 0010 0001 0110 1000 1100 0010 0011 0100 1100 0100 1100 0110 0110 0010 1000 1011 1000"; // page 2-5
            const string bitString2 = "1100 0011 1010 1110 0000 1111 0000 1111 0000 1111 0000 1111 0000";

            TestMakeBitArray(bitString1, 4);
            TestMakeBitArray(bitString2, 4);

            var byteArr = new byte[] { 0x01, 0x02, 0x03, 0x10, 0x11, 0x55, 0xAA, 0xFF };
            string bitString = "0000 0001 0000 0010 0000 0011 0001 0000 0001 0001 0101 0101 1010 1010 1111 1111";

            TestMakeBitArray(bitString, 4, byteArr);

            var ushortArr = new ushort[] { 0x0101, 0x0302, 0x1003, 0x2410, 0x1111, 0x5555, 0xAAAA, 0xFFFF };
            bitString = "00000001 00000001 00000011 00000010 00010000 00000011 00100100 00010000 00010001 00010001 01010101 01010101 10101010 10101010 11111111 11111111";

            TestMakeBitArray(bitString, 8, ushortArr);

            var uintArr = new uint[] { 0x01010101, 0x05040302, 0x30201003, 0x46352410, 0x11111111, 0x55555555, 0xAAAAAAAA, 0xFFFFFFFF };
            bitString = "0000000100000001 0000000100000001 0000010100000100 0000001100000010 0011000000100000 0001000000000011 0100011000110101 0010010000010000 0001000100010001 0001000100010001 0101010101010101 0101010101010101 1010101010101010 1010101010101010 1111111111111111 1111111111111111";

            TestMakeBitArray(bitString, 16, uintArr);
        }

        private void TestMakeBitArray(string bitString, int block)
        {
            var bitArray = MakeBitArray(bitString);
            Assert.AreEqual(bitString, ShowBitArray(bitArray, block, int.MaxValue));
        }

        private void TestMakeBitArray(string bitString, int block, byte[] array)
        {
            var bitArray = MakeBitArray(array);
            Assert.AreEqual(bitString, ShowBitArray(bitArray, block, int.MaxValue));
        }

        private void TestMakeBitArray(string bitString, int block, ushort[] array)
        {
            var bitArray = MakeBitArray(array);
            Assert.AreEqual(bitString, ShowBitArray(bitArray, block, int.MaxValue));
        }

        private void TestMakeBitArray(string bitString, int block, uint[] array)
        {
            var bitArray = MakeBitArray(array);
            Assert.AreEqual(bitString, ShowBitArray(bitArray, block, int.MaxValue));
        }

        [Test]
        [Parallelizable]
        [Category("Support Tests")]
        public void NistRandomnessTests()
        {
            const string bitString1 = "1100 1001 0000 1111 1101 1010 1010 0010 0010 0001 0110 1000 1100 0010 0011 0100 1100 0100 1100 0110 0110 0010 1000 1011 1000"; // page 2-5
            const string bitString2 = "1100 0011 1010 1110 0000 1111 0000 1111 0000 1111 0000 1111 0000";

            var bitArray = MakeBitArray(bitString1);

            double pFreq = FrequencyTest(bitArray);
            Assert.GreaterOrEqual(pFreq, 0.01, $"There is evidence that sequence is NOT random for Frequency test = {pFreq:F4}");

            int blockLength = 8;
            double pBlock = BlockTest(bitArray, blockLength);
            Assert.GreaterOrEqual(pBlock, 0.01, $"There is evidence that sequence is NOT random for Block test = {pBlock:F4}");

            double pRuns = RunsTest(bitArray);
            Assert.GreaterOrEqual(pRuns, 0.01, $"There is evidence that sequence is NOT random for Runs test = {pRuns:F4}");

            bitArray = MakeBitArray(bitString2);

            pFreq = FrequencyTest(bitArray);
            Assert.GreaterOrEqual(pFreq, 0.01, $"There is evidence that sequence is NOT random for Frequency test = {pFreq:F4}");

            blockLength = 8;
            pBlock = BlockTest(bitArray, blockLength);
            Assert.GreaterOrEqual(pBlock, 0.01, $"There is evidence that sequence is NOT random for Block test = {pBlock:F4}");

            //pRuns = RunsTest(bitArray);
            //Assert.GreaterOrEqual(pRuns, 0.01, $"There is evidence that sequence is NOT random for Runs test = {pRuns:F4}");

            var byteArr = new byte[] { 0x01, 0x02, 0x03, 0x10, 0x11, 0x55, 0xAA, 0xFF };
            bitArray = MakeBitArray(byteArr);

            pFreq = FrequencyTest(bitArray);
            Assert.GreaterOrEqual(pFreq, 0.01, $"There is evidence that sequence is NOT random for Frequency test = {pFreq:F4}");

            //blockLength = 8;
            //pBlock = BlockTest(bitArray, blockLength);
            //Assert.GreaterOrEqual(pBlock, 0.01, $"There is evidence that sequence is NOT random for Block test = {pBlock:F4}");

            pRuns = RunsTest(bitArray);
            Assert.GreaterOrEqual(pRuns, 0.01, $"There is evidence that sequence is NOT random for Runs test = {pRuns:F4}");
        }

        private string ShowBitArray(BitArray bitArray, int blockSize, int lineSize)
        {
            var sb = new StringBuilder(bitArray.Length);

            for (int i = 0; i < bitArray.Length; ++i)
            {
                if (i > 0 && i % blockSize == 0)
                {
                    sb.Append(" ");
                }

                if (i > 0 && i % lineSize == 0)
                {
                    sb.AppendLine("");
                }

                if (bitArray[i] == false)
                {
                    sb.Append("0");
                }
                else
                {
                    sb.Append("1");
                }
            }

            return sb.ToString();
        }


        private BitArray MakeBitArray(byte[] list)
        {
            int size = list.Length * 8;
            BitArray result = new BitArray(size); // default is false
            int k = 0; // ptr into result

            for (int index = 0; index < list.Length; ++index)
            {
                result[k] = (list[index] & 0x80) > 0;
                result[k + 1] = (list[index] & 0x40) > 0;
                result[k + 2] = (list[index] & 0x20) > 0;
                result[k + 3] = (list[index] & 0x10) > 0;
                result[k + 4] = (list[index] & 0x08) > 0;
                result[k + 5] = (list[index] & 0x04) > 0;
                result[k + 6] = (list[index] & 0x02) > 0;
                result[k + 7] = (list[index] & 0x01) > 0;

                k += 8;
            }

            return result;
        }

        private BitArray MakeBitArray(ushort[] list)
        {
            int size = list.Length * 2 * 8;
            BitArray result = new BitArray(size); // default is false
            int k = 0; // ptr into result

            for (int index = 0; index < list.Length; ++index)
            {
                result[k] = (list[index] & 0x8000) > 0;
                result[k + 1] = (list[index] & 0x4000) > 0;
                result[k + 2] = (list[index] & 0x2000) > 0;
                result[k + 3] = (list[index] & 0x1000) > 0;
                result[k + 4] = (list[index] & 0x0800) > 0;
                result[k + 5] = (list[index] & 0x0400) > 0;
                result[k + 6] = (list[index] & 0x0200) > 0;
                result[k + 7] = (list[index] & 0x0100) > 0;
                result[k + 8] = (list[index] & 0x0080) > 0;
                result[k + 9] = (list[index] & 0x0040) > 0;
                result[k + 10] = (list[index] & 0x0020) > 0;
                result[k + 11] = (list[index] & 0x0010) > 0;
                result[k + 12] = (list[index] & 0x0008) > 0;
                result[k + 13] = (list[index] & 0x0004) > 0;
                result[k + 14] = (list[index] & 0x0002) > 0;
                result[k + 15] = (list[index] & 0x0001) > 0;

                k += 16;
            }

            return result;
        }

        private BitArray MakeBitArray(uint[] list)
        {
            int size = list.Length * 4 * 8;
            BitArray result = new BitArray(size); // default is false
            int k = 0; // ptr into result

            for (int index = 0; index < list.Length; ++index)
            {
                result[k] = (list[index] & 0x80000000) > 0;
                result[k + 1] = (list[index] & 0x40000000) > 0;
                result[k + 2] = (list[index] & 0x20000000) > 0;
                result[k + 3] = (list[index] & 0x10000000) > 0;
                result[k + 4] = (list[index] & 0x08000000) > 0;
                result[k + 5] = (list[index] & 0x04000000) > 0;
                result[k + 6] = (list[index] & 0x02000000) > 0;
                result[k + 7] = (list[index] & 0x01000000) > 0;
                result[k + 8] = (list[index] & 0x00800000) > 0;
                result[k + 9] = (list[index] & 0x00400000) > 0;
                result[k + 10] = (list[index] & 0x00200000) > 0;
                result[k + 11] = (list[index] & 0x00100000) > 0;
                result[k + 12] = (list[index] & 0x00080000) > 0;
                result[k + 13] = (list[index] & 0x00040000) > 0;
                result[k + 14] = (list[index] & 0x00020000) > 0;
                result[k + 15] = (list[index] & 0x00010000) > 0;
                result[k + 16] = (list[index] & 0x00008000) > 0;
                result[k + 17] = (list[index] & 0x00004000) > 0;
                result[k + 18] = (list[index] & 0x00002000) > 0;
                result[k + 19] = (list[index] & 0x00001000) > 0;
                result[k + 20] = (list[index] & 0x00000800) > 0;
                result[k + 21] = (list[index] & 0x00000400) > 0;
                result[k + 22] = (list[index] & 0x00000200) > 0;
                result[k + 23] = (list[index] & 0x00000100) > 0;
                result[k + 24] = (list[index] & 0x00000080) > 0;
                result[k + 25] = (list[index] & 0x00000040) > 0;
                result[k + 26] = (list[index] & 0x00000020) > 0;
                result[k + 27] = (list[index] & 0x00000010) > 0;
                result[k + 28] = (list[index] & 0x00000008) > 0;
                result[k + 29] = (list[index] & 0x00000004) > 0;
                result[k + 30] = (list[index] & 0x00000002) > 0;
                result[k + 31] = (list[index] & 0x00000001) > 0;

                k += 32;
            }

            return result;
        }

        private BitArray MakeBitArray(string bitString)
        {
            // ex: string "010 101" -> a BitArray of [false,true,false,true,false,true]
            int size = 0;
            for (int i = 0; i < bitString.Length; ++i)
                if (bitString[i] != ' ') ++size;

            BitArray result = new BitArray(size); // default is false
            int k = 0; // ptr into result
            for (int i = 0; i < bitString.Length; ++i)
            {
                if (bitString[i] == ' ') continue;
                if (bitString[i] == '1')
                    result[k] = true;
                else
                    result[k] = false; // not necessary in C#
                ++k;
            }
            return result;
        }

        [Test]
        [Parallelizable]
        [Category("Support Tests")]
        public void RandomGeneratorChiSquareTest()
        {
            const int COUNT = 4096;

            var testRnd = new Random();

            var randomNums = new List<uint>(COUNT);
            Assert.IsFalse(IsRandom(randomNums.ToArray(), 0xFF));

            for (int index = 0; index < COUNT; index++)
            {
                randomNums.Add(unchecked((uint)testRnd.Next(0, 0x100)));
            }

            Assert.IsTrue(IsRandom(randomNums.ToArray(), 0xFF));

            for (int number = 0x00; number <= 0xFF; number++)
            {
                Assert.IsTrue(randomNums.Any(n => n == number));
            }

            Assert.IsFalse(randomNums.Any(n => n == 0x100));

            int counter = 0;

            while (counter < COUNT)
            {
                for (uint number = 0x00; number <= 0xFF; number++)
                {
                    randomNums[counter] = number;
                    counter++;
                }
            }

            Assert.IsFalse(IsRandom(randomNums.ToArray(), 0xFF));
        }

        // Calculates the chi-square value for N positive integers less than r
        // Source: "Algorithms in C" - Robert Sedgewick - pp. 517
        // NB: Sedgewick recommends: "...to be sure, the test should be tried a few times,
        // since it could be wrong in about one out of ten times."
        private bool IsRandom(uint[] randomNums, int r)
        {
            //Calculate the number of samples - N
            int n = randomNums.Length;

            //According to Sedgewick: "This is valid if N is greater than about 10r"
            if (n <= 10 * r)
            {
                return false;
            }

            double n_r = n / r;
            double chi_square = 0;
            Dictionary<uint, uint> table;

            //PART A: Get frequency of randoms
            table = this.RandomFrequency(randomNums);

            //PART B: Calculate chi-square - this approach is in Sedgewick
            double f;

            foreach (var Item in table)
            {
                f = Item.Value - n_r;
                chi_square += Math.Pow(f, 2);
            }

            chi_square = chi_square / n_r;

            double delta = 2 * Math.Sqrt(r);

            //PART C: According to Swdgewick: "The statistic should be within 2(r)^1/2 of r
            //This is valid if N is greater than about 10r"
            if ((r - delta <= chi_square) & (r + delta >= chi_square))
            {
                return true;
            }

            return false;
        }

        //Gets the frequency of occurrence of a randomly generated array of integers
        //Output: A hashtable, key being the random number and value its frequency
        private Dictionary<uint, uint> RandomFrequency(uint[] randomNums)
        {
            Dictionary<uint, uint> table = new Dictionary<uint, uint>();
            int N = randomNums.Length;

            for (int i = 0; i <= N - 1; i++)
            {
                table[randomNums[i]] = table.ContainsKey(randomNums[i]) ? table[randomNums[i]] + 1 : 1;
            }

            return table;
        }

        private double FrequencyTest(BitArray bitArray)
        {
            // perform a NIST frequency test on bitArray
            double sum = 0;

            for (int i = 0; i < bitArray.Length; ++i)
            {
                sum = bitArray[i] == false ? sum - 1 : sum + 1;
            }

            double testStat = Math.Abs(sum) / Math.Sqrt(bitArray.Length);
            double rootTwo = 1.414213562373095;
            double pValue = ErrorFunctionComplement(testStat / rootTwo);
            return pValue;
        }

        private double ErrorFunction(double x)
        {
            // assume x > 0.0
            // Abramowitz and Stegun eq. 7.1.26
            double p = 0.3275911;
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double t = 1.0 / (1.0 + p * x);
            double err = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return err;
        }

        private double ErrorFunctionComplement(double x)
        {
            return 1 - ErrorFunction(x);
        }

        private double BlockTest(BitArray bitArray, int blockLength)
        {
            // NIST intra-block frequency test
            int numBlocks = bitArray.Length / blockLength; // 'N'

            double[] proportions = new double[numBlocks];
            int k = 0; // ptr into bitArray

            for (int block = 0; block < numBlocks; ++block)
            {
                int countOnes = 0;

                for (int i = 0; i < blockLength; ++i)
                {
                    if (bitArray[k++] == true)
                    {
                        ++countOnes;
                    }
                }

                proportions[block] = (countOnes * 1.0) / blockLength;
            }

            double summ = 0.0;

            for (int block = 0; block < numBlocks; ++block)
            {
                summ = summ + (proportions[block] - 0.5) * (proportions[block] - 0.5);
            }

            double chiSquared = 4 * blockLength * summ;

            double a = numBlocks / 2.0;
            double x = chiSquared / 2.0;
            double pValue = GammaFunctions.GammaUpper(a, x);

            return pValue;
        }

        private double RunsTest(BitArray bitArray)
        {
            // NIST Runs test
            double numOnes = 0.0;

            for (int i = 0; i < bitArray.Length; ++i)
            {
                if (bitArray[i] == true)
                {
                    ++numOnes;
                }
            }

            double prop = (numOnes * 1.0) / bitArray.Length;

            //double tau = 2.0 / Math.Sqrt(bitArray.Length * 1.0);
            //if (Math.Abs(prop - 0.5) >= tau)
            //  return 0.0; // not-random short-circuit

            int runs = 1;

            for (int i = 0; i < bitArray.Length - 1; ++i)
            {
                if (bitArray[i] != bitArray[i + 1])
                {
                    ++runs;
                }
            }

            double num = Math.Abs(runs - (2 * bitArray.Length * prop * (1 - prop)));
            double denom = 2 * Math.Sqrt(2.0 * bitArray.Length) * prop * (1 - prop);
            double pValue = ErrorFunctionComplement(num / denom);

            return pValue;
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_DRW_Vx_Vy_nibble()
        {
            Assert.Inconclusive();
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SKP_Vx()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xE0 | (registerX & 0x0F)));
                emptyMemory[testAddress + 1] = 0x9E;
            }

            Test_SKP_Vx(emptyMemory, testAddress);
        }

        private void Test_SKP_Vx(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                ushort opcode = unchecked((ushort)((0xE0 | (registerX & 0x0F)) << 8 | 0x9E));

                for (int testKey = 0x00; testKey <= 0xFF; testKey++)
                {
                    for (byte testKeyValue = 0x00; testKeyValue < 0x10; testKeyValue++)
                    {
                        cpu.PC = address;
                        cpu.V[registerX] = unchecked((byte)testKey);

                        byte value = unchecked((byte)(testKey << 1));
                        byte carryFlag = unchecked((byte)((testKey & 0x80) >> 7));

                        for (int key = 0; key <= 0xF; key++)
                        {
                            cpu.keys[key] = unchecked((byte)((key == testKeyValue) ? 1 : 0));
                        }

                        if (cpu.V[registerX] > 0x0F)
                        {
                            Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                            continue;
                        }
                        else
                        {
                            cpu.EmulateCycle();
                        }

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SKNP V{registerX:X1}");

                        if (testKey == testKeyValue)
                        {
                            Assert.AreEqual(address + 4, cpu.PC, $"Should skip the next instruction when V{registerX:X1} key is pressed (Key 0x{testKey:X2}= 0x{cpu.keys[testKey]:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}]");
                        }
                        else
                        {
                            Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when V{registerX:X1} key is not pressed (Key 0x{testKey:X2}= 0x{cpu.keys[testKey]:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}]");
                        }
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_SKNP_Vx()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xE0 | (registerX & 0x0F)));
                emptyMemory[testAddress + 1] = 0xA1;
            }

            Test_SKNP_Vx(emptyMemory, testAddress);
        }

        private void Test_SKNP_Vx(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;
                ushort opcode = unchecked((ushort)((0xE0 | (registerX & 0x0F)) << 8 | 0xA1));

                for (int testKey = 0x00; testKey <= 0xFF; testKey++)
                {
                    for (byte testKeyValue = 0x00; testKeyValue < 0x10; testKeyValue++)
                    {
                        cpu.PC = address;
                        cpu.V[registerX] = unchecked((byte)testKey);

                        byte value = unchecked((byte)(testKey << 1));
                        byte carryFlag = unchecked((byte)((testKey & 0x80) >> 7));

                        for (int key = 0; key < 0x10; key++)
                        {
                            cpu.keys[key] = unchecked((byte)((key == testKeyValue) ? 1 : 0));
                        }

                        if (cpu.V[registerX] > 0x0F)
                        {
                            Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                            continue;
                        }
                        else
                        {
                            cpu.EmulateCycle();
                        }

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - SKNP V{registerX:X1}");

                        if (testKey != testKeyValue)
                        {
                            Assert.AreEqual(address + 4, cpu.PC, $"Should skip the next instruction when V{registerX:X1} key is not pressed (Key 0x{testKey:X2}= 0x{cpu.keys[testKey]:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}]");
                        }
                        else
                        {
                            Assert.AreEqual(address + 2, cpu.PC, $"Should not skip the next instruction when V{registerX:X1} key is pressed (Key 0x{testKey:X2}= 0x{cpu.keys[testKey]:X2}) [opcode: 0x{opcode:X4}] [V{registerX:X1}= 0x{cpu.V[registerX]:X2}]");
                        }
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_Vx_DT()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xF0 | (registerX & 0x0F)));
                emptyMemory[testAddress + 1] = 0x07;
            }

            Test_LD_Vx_DT(emptyMemory, testAddress);
        }

        private void Test_LD_Vx_DT(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;

                ushort opcode = unchecked((ushort)((0xF0 | (registerX & 0x0F)) << 8 | 0x07));

                for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                {
                    cpu.PC = address;
                    byte invertedValue = unchecked((byte)~testValue);
                    cpu.DT = unchecked((byte)testValue);
                    cpu.V[registerX] = invertedValue;

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - LD V{registerX:X1}, DT");

                    for (int index = 0; index <= 0xF; index++)
                    {
                        if (index == registerX)
                        {
                            Assert.AreNotEqual(invertedValue, cpu.V[registerX], $"V{registerX:X1} should not be 0x{invertedValue:X2}");
                            Assert.AreEqual(testValue, cpu.V[registerX], $"V{registerX:X1} should be equal DT (0x{testValue:X2}) [V{registerX:X1}=0x{cpu.V[registerX]:X2}]");
                        }
                        else
                        {
                            Assert.AreEqual(0x00, cpu.V[index], $"V{index:X1} should be equal 0x00 while testing register V{registerX:X1} for 0x{testValue:X2} [V{index:X1}=0x{cpu.V[index]:X2}]");
                        }
                    }

                    cpu.V[registerX] = 0x00;
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_DT_Vx()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xF0 | (registerX & 0x0F)));
                emptyMemory[testAddress + 1] = 0x15;
            }

            Test_LD_DT_Vx(emptyMemory, testAddress);
        }

        private void Test_LD_DT_Vx(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;

                ushort opcode = unchecked((ushort)((0xF0 | (registerX & 0x0F)) << 8 | 0x15));

                for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                {
                    cpu.PC = address;
                    byte invertedValue = unchecked((byte)~testValue);
                    cpu.DT = invertedValue;
                    cpu.V[registerX] = unchecked((byte)testValue);

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - LD DT, V{registerX:X1}");

                    if (testValue != 0x80)
                    {
                        Assert.AreNotEqual(invertedValue, cpu.DT, $"DT should not be 0x{invertedValue:X2}");
                    }

                    if (testValue == 0)
                    {
                        Assert.AreEqual(0x00, cpu.DT, $"DT should be equal V{registerX:X1} - 1 (0x{testValue:X2})");
                    }
                    else
                    {
                        Assert.AreEqual(testValue - 1, cpu.DT, $"DT should be equal V{registerX:X1} - 1 (0x{testValue:X2})");
                    }

                    cpu.V[registerX] = 0x00;
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_ST_Vx()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xF0 | (registerX & 0x0F)));
                emptyMemory[testAddress + 1] = 0x18;
            }

            Test_LD_ST_Vx(emptyMemory, testAddress);
        }

        private void Test_LD_ST_Vx(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int registerX = cpu.memory[address] & 0x0F;

                ushort opcode = unchecked((ushort)((0xF0 | (registerX & 0x0F)) << 8 | 0x18));

                for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                {
                    cpu.PC = address;
                    byte invertedValue = unchecked((byte)~testValue);
                    cpu.ST = invertedValue;
                    cpu.V[registerX] = unchecked((byte)testValue);

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - LD ST, V{registerX:X1}");

                    if (testValue != 0x80)
                    {
                        Assert.AreNotEqual(invertedValue, cpu.ST, $"ST should not be 0x{invertedValue:X2}");
                    }

                    if (testValue == 0)
                    {
                        Assert.AreEqual(0x00, cpu.ST, $"ST should be equal V{registerX:X1} - 1 (0x{testValue:X2})");
                    }
                    else
                    {
                        Assert.AreEqual(testValue - 1, cpu.ST, $"ST should be equal V{registerX:X1} - 1 (0x{testValue:X2})");
                    }

                    cpu.V[registerX] = 0x00;
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_Vx_K()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xF0 | (registerX & 0x0F)));
                emptyMemory[testAddress + 1] = 0x0A;
            }

            Test_LD_Vx_K(emptyMemory, testAddress);
        }

        private void Test_LD_Vx_K(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int register = cpu.memory[address] & 0x0F;
                ushort opcode = unchecked((ushort)((0xF0 | (register & 0x0F)) << 8 | 0x0A));

                for (byte testKeyValue = 0x00; testKeyValue < 0x10; testKeyValue++)
                {
                    cpu.PC = address;
                    cpu.V[register] = unchecked((byte)~testKeyValue);

                    for (int key = 0; key <= 0xF; key++)
                    {
                        cpu.keys[key] = unchecked((byte)((key == testKeyValue) ? 1 : 0));
                    }

                    cpu.EmulateCycle();

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - LD V{register:X1}, K");
                    Assert.AreEqual(testKeyValue, cpu.V[register], $"V{register:X1} should be the index of the pressed key (Key 0x{testKeyValue:X2}=0x{cpu.keys[testKeyValue]:X2}) [opcode: 0x{opcode:X4}] [V{register:X1}= 0x{cpu.V[register]:X2}]");
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_ADD_I_Vx()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int register = 0; register <= 0xF; register++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{register}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xF0 | (register & 0x0F)));
                emptyMemory[testAddress + 1] = 0x1E;
            }

            Test_ADD_I_Vx(emptyMemory, testAddress);
        }

        private void Test_ADD_I_Vx(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int register = cpu.memory[address] & 0x0F;
                byte value = cpu.memory[address + 1];
                byte invertedValue = unchecked((byte)~value);
                ushort opcode = unchecked((ushort)((0xF0 | (register & 0x0F)) << 8 | 0x1E));

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = invertedValue;
                }

                cpu.PC = address;
                cpu.I = 0x000;
                cpu.EmulateCycle();

                for (int index = 0; index <= 0xF; index++)
                {
                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - ADD I, V{register:X1}");

                    Assert.AreNotEqual(value, cpu.V[index], $"V{index:X1} should not be 0x{value:X2}");
                    Assert.AreEqual(invertedValue, cpu.V[index], $"V{index:X1} should be 0x{invertedValue:X2}");

                    cpu.V[index] = invertedValue;
                }

                for (ushort baseAddr = 0x000; baseAddr < 0x1000; baseAddr++)
                {
                    for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                    {
                        cpu.PC = address;
                        cpu.V[register] = unchecked((byte)testValue);
                        cpu.I = baseAddr;


                        ushort destAddr = unchecked((ushort)(baseAddr + testValue));

                        if (destAddr > 0xFFF)
                        {
                            Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                        }
                        else
                        {
                            cpu.EmulateCycle();

                            Assert.AreEqual(destAddr, cpu.I, $"Register I should be {destAddr} (0x{baseAddr:X3} + 0x{testValue:X2}) [opcode: 0x{opcode:X4}] [V{register:X1}= 0x{cpu.V[register]:X2}]");
                        }

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - ADD I, V{register:X1}");
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_F_Vx()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xF0 | (registerX & 0x0F)));
                emptyMemory[testAddress + 1] = 0x29;
            }

            Test_LD_F_Vx(emptyMemory, testAddress);
        }

        private void Test_LD_F_Vx(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                int register = cpu.memory[address] & 0x0F;
                ushort opcode = unchecked((ushort)((0xF0 | (register & 0x0F)) << 8 | 0x29));

                for (int testValue = 0x00; testValue <= 0xFF; testValue++)
                {
                    cpu.PC = address;
                    byte invertedValue = unchecked((byte)~testValue);
                    cpu.V[register] = unchecked((byte)testValue);

                    if (testValue > 0x0F)
                    {
                        Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                    }
                    else
                    {
                        cpu.EmulateCycle();

                        Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - LD F, V{register:X1}");

                        for (int index = 0; index <= 0xF; index++)
                        {
                            if (index == register)
                            {
                                Assert.AreEqual(testValue, cpu.V[register], $"V{register:X1} should be 0x{invertedValue:X2}");
                                Assert.AreEqual(testValue * 5, cpu.I, $"I should point to address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");

                                switch (testValue)
                                {
                                    case 0x0:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0x1:
                                        Assert.AreEqual(0x20, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x60, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x20, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x20, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x70, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0x2:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x10, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0x3:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x10, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x10, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0x4:
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x10, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x10, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0x5:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x10, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0x6:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0x7:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x10, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x20, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x40, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x40, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0x8:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0x9:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x10, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0xA:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0xB:
                                        Assert.AreEqual(0xE0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xE0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xE0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0xC:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0xD:
                                        Assert.AreEqual(0xE0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x90, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xE0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0xE:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                    case 0xF:
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 1], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 1:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0xF0, cpu.memory[cpu.I + 2], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 2:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 3], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X3} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        Assert.AreEqual(0x80, cpu.memory[cpu.I + 4], $"Problem on character 0x{testValue:X1} at address 0x{testValue * 5 + 3:X4} [V{register:X1}=0x{cpu.V[register]:X2}]");
                                        break;
                                }
                            }
                            else
                            {
                                Assert.AreEqual(0x00, cpu.V[index], $"V{index:X1} should be equal 0x00 while testing register V{register:X1} for 0x{testValue:X2} [V{index:X1}=0x{cpu.V[index]:X2}]");
                            }
                        }
                    }

                    cpu.V[register] = 0x00;
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_B_Vx()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xF0 | (registerX & 0x0F)));
                emptyMemory[testAddress + 1] = 0x33;
            }

            Test_LD_B_Vx(emptyMemory, testAddress);
        }

        private void Test_LD_B_Vx(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {

                int register = cpu.memory[address] & 0x0F;

                ushort opcode = unchecked((ushort)((0xF0 | (register & 0x0F)) << 8 | 0x33));

                for (ushort baseAddr = 0x000; baseAddr < 0x1000; baseAddr++)
                {
                    if ((address >= baseAddr && address <= baseAddr + 2) ||
                        (address + 1 >= baseAddr && address + 1 <= baseAddr + 2))
                    {
                        continue;
                    }

                    cpu.LoadMemory(emptyMemory);

                    for (int value = 0x00; value <= 0xFF; value++)
                    {
                        cpu.PC = address;
                        cpu.I = baseAddr;
                        cpu.V[register] = unchecked((byte)value);

                        if (baseAddr < 0x200 || baseAddr + 2 > 0xFFF)
                        {
                            Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                        }
                        else
                        {
                            cpu.EmulateCycle();

                            Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - LD B, V{register:X1} at 0x{cpu.PC:X3} [I=0x{cpu.I:X3}]");

                            for (int index = 0; index <= 0xF; index++)
                            {
                                if (index == register)
                                {
                                    int cents = value / 100;
                                    int tens = (value % 100) / 10;
                                    int units = (value % 100) % 10;

                                    Assert.AreEqual(value, cpu.V[register], $"V{register:X1} should not change");
                                    Assert.AreEqual(cents, cpu.memory[cpu.I], $"memory address 0x{cpu.I:X3} should be 0x{cents:X2}");
                                    Assert.AreEqual(tens, cpu.memory[cpu.I + 1], $"memory address 0x{cpu.I + 1:X1} should be 0x{tens:X2}");
                                    Assert.AreEqual(units, cpu.memory[cpu.I + 2], $"memory address 0x{cpu.I + 2:X1} should be 0x{units:X2}");
                                }
                                else
                                {
                                    Assert.AreEqual(0x00, cpu.V[index], $"V{index:X1} should be equal 0x00 while testing register V{register:X1} for 0x{value:X2} [V{index:X1}=0x{cpu.V[index]:X2}]");
                                }
                            }
                        }

                        cpu.V[register] = 0x00;
                    }
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_IPtr_Vx()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xF0 | (registerX & 0x0F)));
                emptyMemory[testAddress + 1] = 0x55;
            }

            Test_LD_IPtr_Vx(emptyMemory, testAddress);
        }

        private void Test_LD_IPtr_Vx(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                cpu.LoadMemory(emptyMemory);

                int register = cpu.memory[address] & 0x0F;
                byte value = unchecked((byte)rnd.Next(0x00, 0x100));
                byte invertedValue = unchecked((byte)~value);
                ushort opcode = unchecked((ushort)((0xF0 | (register & 0x0F)) << 8 | 0x55));

                for (int index = 0; index <= 0xF; index++)
                {
                    cpu.V[index] = unchecked((byte)(value + index));
                }

                for (ushort baseAddr = 0x000; baseAddr < 0x1000; baseAddr++)
                {
                    cpu.I = baseAddr;

                    ushort destAddr = unchecked((ushort)(baseAddr + register));

                    if ((address >= baseAddr && address <= destAddr) ||
                        (address + 1 >= baseAddr && address + 1 <= destAddr))
                    {
                        continue;
                    }

                    cpu.PC = address;

                    if (destAddr > 0xFFF || destAddr < 0x200)
                    {
                        Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                    }
                    else
                    {
                        for (byte index = 0; index <= register; index++)
                        {
                            cpu.memory[cpu.I + index] = invertedValue;
                        }

                        cpu.EmulateCycle();

                        for (byte index = 0; index <= register; index++)
                        {
                            Assert.AreEqual(cpu.V[index], cpu.memory[cpu.I + index], $"Memory position at address 0x{cpu.I:X3} should be 0x{cpu.V[index]:X2} [opcode: 0x{opcode:X4}]");
                        }

                        for (int index = 0; index <= 0xF; index++)
                        {
                            Assert.AreEqual((byte)(value + index), cpu.V[index], $"V{index:X1} should not change");
                        }
                    }

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - LD [I], V{register:X1}");
                }
            }
        }

        [Test]
        [Parallelizable]
        [Category("Opcode Tests")]
        public void Opcode_LD_Vx_IPtr()
        {
            var emptyMemory = new byte[0x1000];
            int testAddress = 0x200;

            for (int registerX = 0; registerX <= 0xF; registerX++, testAddress += 2)
            {
                if (testAddress >= 0x1000)
                {
                    Assert.Fail($"[{testAddress}]: V{registerX}");
                    break;
                }

                emptyMemory[testAddress] = unchecked((byte)(0xF0 | (registerX & 0x0F)));
                emptyMemory[testAddress + 1] = 0x65;
            }

            Test_LD_Vx_IPtr(emptyMemory, testAddress);
        }

        private void Test_LD_Vx_IPtr(byte[] emptyMemory, int testAddress)
        {
            var cpu = new CPU();
            cpu.LoadMemory(emptyMemory);

            Assert.AreEqual(0x200, cpu.PC, "PC Register should be at 0x200");

            for (int index = 0; index <= 0xF; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X1} should be zero.");
            }

            for (ushort address = 0x200; address < testAddress; address += 2)
            {
                cpu.LoadMemory(emptyMemory);

                int register = cpu.memory[address] & 0x0F;
                byte value = unchecked((byte)rnd.Next(0x00, 0x100));
                byte invertedValue = unchecked((byte)~value);
                ushort opcode = unchecked((ushort)((0xF0 | (register & 0x0F)) << 8 | 0x65));

                for (ushort baseAddr = 0x000; baseAddr < 0x1000; baseAddr++)
                {
                    ushort destAddr = unchecked((ushort)(baseAddr + register));

                    cpu.I = baseAddr;

                    if ((address >= baseAddr && address <= destAddr) ||
                        (address + 1 >= baseAddr && address + 1 <= destAddr))
                    {
                        continue;
                    }

                    cpu.PC = address;

                    if (destAddr > 0xFFF)
                    {
                        Assert.Throws<InvalidOperationException>(cpu.EmulateCycle);
                    }
                    else
                    {
                        for (ushort index = 0; index <= register; index++)
                        {
                            cpu.memory[cpu.I + index] = unchecked((byte)(value + index));
                        }

                        for (int index = 0; index <= 0xF; index++)
                        {
                            cpu.V[index] = invertedValue;
                        }

                        cpu.EmulateCycle();

                        for (byte index = 0; index <= register; index++)
                        {
                            Assert.AreEqual((byte)(value + index), cpu.V[index], $"V{index:X1} should be 0x{cpu.memory[cpu.I + index]:X2} [opcode: 0x{opcode:X4}] [I=0x{cpu.I:X3}] [PC=0x{cpu.PC:X3}]");
                        }

                        for (byte index = 0; index <= register; index++)
                        {
                            Assert.AreEqual((byte)(value + index), cpu.memory[cpu.I + index], $"Memory position 0x{cpu.I + index:X3} should not change");
                        }
                    }

                    Assert.AreEqual(opcode, cpu.opcode, $"Opcode shoud be 0x{opcode:X4} - LD V{register:X1}, [I]");
                }
            }
        }
    }
}
