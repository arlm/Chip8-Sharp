### Opcode table

CHIP-8 has 35 opcodes, which are all two bytes long and stored big-endian. 
The opcodes are listed below, in hexadecimal and with the following symbols:

*   NNN: address
*   NN: 8-bit constant
*   N: 4-bit constant
*   X and Y: 4-bit register identifier
*   PC : Program Counter
*   I : 16bit register (For memory address) (Similar to void pointer)

|-+------+-------+-----------------+---------------------------------------------|
| |Opcode|  Type |     C Pseudo    |Explanation                                  |
|-+------+-------+-----------------+---------------------------------------------|
| |0x0NNN|Call   |                 |Calls RCA 1802 program at address NNN.       |
| |      |       |                 |  Not necessary for most ROMs.               |
|X|0x00E0|Display|disp_clear()     |Clears the screen.                           |
|X|0x00EE|Flow   |return;          |Returns from a subroutine.                   |
|X|0x1NNN|Flow   |goto NNN;        |Jumps to address NNN.                        |
|X|0x2NNN|Flow   |*(0xNNN)()       |Calls subroutine at NNN.                     |
|X|0x3XNN|Cond   |if(Vx==NN)       |Skips the next instruction if VX equals NN.  |
| |      |       |                 |  (Usually the next instruction is a jump to |
| |      |       |                 |   skip a code block)                        |
|X|0x4XNN|Cond   |if(Vx!=NN)       |Skips the next instruction if VX doesn't     |
| |      |       |                 |  equal NN. (Usually the next instruction is |
| |      |       |                 |  a jump to skip a code block)               |
|X|0x5XY0|Cond   |if(Vx==Vy)       |Skips the next instruction if VX equals VY.  |
| |      |       |                 |  (Usually the next instruction is a jump to |
| |      |       |                 |   skip a code block)                        |
|X|0x6XNN|Const  |Vx = NN          |Sets VX to NN.                               |
|X|0x7XNN|Const  |Vx += NN         |Adds NN to VX. (Carry flag is not changed)   |
|X|0x8XY0|Assign |Vx = Vy          |Sets VX to the value of VY.                  |
|X|0x8XY1|BitOp  |Vx = Vx | Vy     |Sets VX to VX or VY. (Bitwise OR operation)  |
|X|0x8XY2|BitOp  |Vx = Vx & Vy     |Sets VX to VX and VY. (Bitwise AND operation)|
|X|0x8XY3|BitOp  |Vx = Vx ^ Vy     |Sets VX to VX xor VY.                        |
|X|0x8XY4|Math   |Vx += Vy         |Adds VY to VX. VF is set to 1 when there's   |
| |      |       |                 |  a carry, and to 0 when there isn't.        |
|X|0x8XY5|Math   |Vx -= Vy         |VY is subtracted from VX. VF is set to 0 when|
| |      |       |                 |  there's a borrow, and 1 when there isn't.  |
|X|0x8XY6|BitOp  |Vx >>= 1         |Stores the least significant bit of VX in VF |
| |      |       |                 |  and then shifts VX to the right by 1.[2]   |
|X|0x8XY7|Math   |Vx = Vy - Vx     |Sets VX to VY minus VX. VF is set to 0 when  |
| |      |       |                 |  there's a borrow, and 1 when there isn't.  |
|X|0x8XYE|BitOp  |Vx <<= 1         |Stores the most significant bit of VX in VF  |
| |      |       |                 |  and then shifts VX to the left by 1.[3]    |
|X|0x9XY0|Cond   |if(Vx!=Vy)       |Skips the next instruction if VX doesn't     |
| |      |       |                 |  equal VY. (Usually the next instruction is |
| |      |       |                 |  a jump to skip a code block)               |
|X|0xANNN|MEM    |I = NNN          |Sets I to the address NNN.                   |
|X|0xBNNN|Flow   |PC= V0 + NNN     |Jumps to the address NNN plus V0.            |
|X|0xCXNN|Rand   |Vx= rand() & NN  |Sets VX to the result of a bitwise and       |
| |      |       |                 |  operation on a random number               |
| |      |       |                 |  (Typically: 0 to 255) and NN.              |
|X|0xDXYN|Disp   |draw(Vx,Vy,N)    |Draws a sprite at coordinate (VX, VY) that   |
| |      |       |                 |  has a width of 8 pixels and a height of    |
| |      |       |                 |  N pixels. Each row of 8 pixels is read as  |
| |      |       |                 |  bit-coded starting from memory location I; |
| |      |       |                 |  I value doesn’t change after the execution |
| |      |       |                 |  of this instruction. As described above,   |
| |      |       |                 |  VF is set to 1 if any screen pixels are    |
| |      |       |                 |  flipped from set to unset when the sprite  |
| |      |       |                 |  is drawn, and to 0 if that doesn’t happe   |
|X|0xEX9E|KeyOp  |if(key()==Vx)    |Skips the next instruction if the key stored |
| |      |       |                 |  in VX is pressed. (Usually the next        |
| |      |       |                 |  instruction is a jump to skip a code block)|
|X|0xEXA1|KeyOp  |if(key()!=Vx)    |Skips the next instruction if the key stored |
| |      |       |                 |  in VX isn't pressed. (Usually the next     |
| |      |       |                 |  instruction is a jump to skip a code block)|
|X|0xFX07|Timer  |Vx = get_delay() |Sets VX to the value of the delay timer.     |
|X|0xFX0A|KeyOp  |Vx = get_key()   |A key press is awaited, and then stored in   |
| |      |       |                 |  VX. (Blocking Operation. All instruction   |
| |      |       |                 |  halted until next key event)               |
|X|0xFX15|Timer  |delay_timer(Vx)  |Sets the delay timer to VX.                  |
|X|0xFX18|Sound  |sound_timer(Vx)  |Sets the sound timer to VX.                  |
|X|0xFX1E|MEM    |I +=Vx           |Adds VX to I.[4]                             |
|X|0xFX29|MEM    |I=sprite_addr[Vx]|Sets I to the location of the sprite for the |
| |      |       |                 |  character in VX. Characters 0x0-0xF are    |
| |      |       |                 |  represented by a 4x5 font.                 |
|X|0xFX33|BCD    |set_BCD(Vx);     |Stores the binary-coded decimal              |
| |      |       |*(I+0) = BCD(3); |  representation of VX, with the most        |
| |      |       |*(I+1) = BCD(2); |  significant of three digits at the address |
| |      |       |*(I+2) = BCD(1); |  in I, the middle digit at I plus 1, and the|
| |      |       |                 |  least significant digit at I plus 2.       |
| |      |       |                 |  (In other words, take the decimal          |
| |      |       |                 |  representation of VX, place the hundreds   |
| |      |       |                 |  digit in memory at location in I, the tens |
| |      |       |                 |  digit at location I+1, and the ones digit  |
| |      |       |                 |  at location I+2.)                          |
|X|0xFX55|MEM    |reg_dump(Vx,&I)  |Stores V0 to VX (including VX) in memory     |
| |      |       |                 |  starting at address I. The offset from I is|
| |      |       |                 |  increased by 1 for each value written,     |
| |      |       |                 |  but I itself is left unmodified.           |
|X|0xFX65|MEM    |reg_load(Vx,&I)  |Fills V0 to VX (including VX) with values    |
| |      |       |                 |  from memory starting at address I.         |
| |      |       |                 |  The offset from I is increased by 1 for    |
| |      |       |                 |  each value written, but I itself is left   |
| |      |       |                 |  unmodified.                                |
|-+------+-------+-----------------+---------------------------------------------|

^2 ["Cowgod's Chip-8 Technical Reference"](http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#8xy6)
^3 ["Cowgod's Chip-8 Technical Reference"](http://devernay.free.fr/hacks/chip8/C8TECH10.HTM#8xyE)
^4 VF is set to 1 when there is a range overflow (I+VX>0xFFF), and to 0 when there isn't.
   This is an undocumented feature of the CHIP-8 and used by the Spacefight 2091! game.