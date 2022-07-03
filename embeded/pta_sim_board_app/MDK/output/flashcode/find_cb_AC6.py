import os

MAPFILE = 'flashcode.map'

def check_p0(file_name):
    t = ".bss._SEGGER_RTT    segger_rtt.o"
    p0_list = []
    with open(file_name, 'r') as f:
        for line in f:
            if line.find(t) >= 0:
                pos = line.find('-')
                p0 = line[0:pos].strip()
                p0_list.append(p0)

    print('cb block address: ' + str(p0_list[0]))

    portion = os.path.splitext(file_name)
    newName = portion[0] + ".txt"    
    with open(newName, 'w+') as f:
        f.writelines('cb block address: ' + str(p0_list[0]))

def check_all():
    print('--------------cb address---------------')
    check_p0(MAPFILE)

check_all()

