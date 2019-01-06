using System;
using Chip8.Core;
using NUnit.Framework;

namespace Chip8.Tests
{
    [TestFixture]
    public class TestCPU
    {
        [Test]
        public void InitialState()
        {
            var emptyMemory = new byte[0x1000] ;
            var cpu = new CPU();

            Assert.AreEqual(0x1000, cpu.memory.Length, "Memory should be exactly 4KiB long.");
            Assert.AreEqual(16, cpu.V.Length, "The CPU must have exactl 16 V registers.");
            CollectionAssert.AllItemsAreInstancesOfType(cpu.V, typeof(byte), "The CPU must have 8-bit registers.");

            for (int index = 0; index < cpu.V.Length; index++)
            {
                Assert.Zero(cpu.V[index], $"Register V{index:X} should be zero.");
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
                Assert.Zero(cpu.stack[index], $"Stack position {index:X} should be zero.");
            }

            Assert.AreEqual(16, cpu.keys.Length, "The CPU must have exactly 16 keys.");
            CollectionAssert.AllItemsAreInstancesOfType(cpu.keys, typeof(byte), "The keys registers must be 8-bit elements.");

            for (int index = 0; index < cpu.keys.Length; index++)
            {
                Assert.Zero(cpu.keys[index], $"Key {index:X} should be zero.");
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

            for (int index = 0; index < cpu.memory.Length; index++)
            {
                Assert.Zero(cpu.memory[index], $"Memory address {index:X} should be zero.");
            }
        }
    }
}
