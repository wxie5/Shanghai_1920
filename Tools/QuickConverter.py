

if __name__ == '__main__':
    name = 'Ch1.txt'
    with open('out.txt', 'w', encoding='utf-8') as output:
        with open(name, 'r', encoding='utf-8') as f:
            extra = ''
            has_extra = False
            for line in f.readlines():
                column = line.find(':')
                if column == -1:
                    extra += line
                    has_extra = True
                    # print(f'line: {line}')
                else:
                    if has_extra:
                        output.write(f'/***\n{extra}***/\n')
                        extra = ''
                        has_extra = False
                    name = line[:column]
                    words = line[column+2:-1]
                    is_self = name.find('Liu Xingzhe') != -1 and name.find('Narrator') != -1
                    if is_self:
                        is_self_talking = 't' if name.find('Liu Xingzhe') != -1 else 'f'
                        output.write(f'M|{is_self_talking}|{words}|n|f|*|*\n')
                    else:
                        output.write(f'T|{name}|{words}|*|n|f|0|f|*|*\n')
