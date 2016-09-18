org 0
ldi R3, $87
ldi r3, 1(r3)
ld r2, 0x66
ldi r2, -1(r2)
ldr r7, 0x61
str 0x60, r7
ld r1, 0(r2)
ldi r0, 1
nop
add r3, r2, r3
addi r7, r7, 2
neg r7, r7
not r7, r7
andi r7, r7, 0x0f
ori r7, r1, 3
shr r2, r3, r0
st 0x56, r2
ror r1, r1, r0
rol r2, r2, r0
or r2, r3, r0
and r1, r2, r1
st 0x4c(r1), r3
sub r3,r2,r3
shl r1, r2, r0
ldi r4, 5
ldi r5, 0x1f
mul r5,r4
mfhi r7
mflo r6
div r5, r4
ldi r10, 0(r4)
ldi r11, 0(r5)
ldi r12, 0(r6)
ldi r13, 0(r7)
jal r12
in r4
st 0x90, r4
ldi r1, 0x2a
ldi r2, 0x2e
ldi r3, 0x35
ldi r7, 1
ldi r5, 40

@loop
out r4
ldi r5, -1(r5)
brzr r5, r3
ld r6, 0xf0

@loop2
ldi r6, -1(r6)
nop
brnz r6, r2
shr r4, r4, r7
brnz r4, r1
ld r4, 0x90
jr r1

@done
ldi r4, 0xa5
out r4
halt





org 0x56

&0x34

org 0x66
&0x57

org 0x9B
@subA

add r9, r10, r12
sub r8, r11, r13
sub r9, r9, r8
jr r14


org 0xf0
&0xFFFF