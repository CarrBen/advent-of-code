#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stddef.h>
#include <stdbool.h>
#include <limits.h>
#include <math.h>

#define DEBUG_INST (false)
#define DEBUG_GOOD_INPUTS (true)

enum inst {
    INPUT,
    ADD,
    MUL,
    DIV,
    MOD,
    EQL
};

enum valueType {
    CONST,
    VAR
};

union value {
    char var;
    int constant;
};

struct instruction {
    enum inst type;
    enum valueType aType;
    union value a;
    enum valueType bType;
    union value b;
};

struct instruction parseLine(char *line) {
    struct instruction i = { 0 };
    
    switch(line[1]) {
        case 'd':
            i.type = ADD;
            break;
        case 'n':
            i.type = INPUT;
            break;
        case 'u':
            i.type = MUL;
            break;
        case 'i':
            i.type = DIV;
            break;
        case 'o':
            i.type = MOD;
            break;
        case 'q':
            i.type = EQL;
            break;
        default:
            printf("Bad instruction %s\n", line);
            break;
    }
    
    strtok(line, " ");
    char *a = strtok(NULL, " ");
    char *b = strtok(NULL, " ");
    
    if(a[0] > '9') {
        i.aType = VAR;
        i.a.var = a[0];
    }else{
        i.aType = CONST;
        sscanf(a, "%d", &i.a.constant);
    }
    
    // inp has only 1 value
    if(i.type == INPUT) return i;
    
    if(b[0] > '9') {
        i.bType = VAR;
        i.b.var = b[0];
    }else{
        i.bType = CONST;
        sscanf(b, "%d", &i.b.constant);
    }
    
    return i;
}

struct state {
    int x;
    int y;
    int z;
    int w;
};

void printState(struct state s) {
    printf("x: %d, y: %d, z: %d, w: %d\n", s.x, s.y, s.z, s.w);
}

int fieldOffset(char var) {
    switch(var) {
        case 'x':
            return 0;
        case 'y':
            return 1;
        case 'z':
            return 2;
        case 'w':
            return 3;
        default:
            return -1;
    };
}

void execute(struct instruction i, struct state *s, int input) {
    if(i.type == INPUT) {
        if(i.aType == CONST) {
            printf("Bad inp with CONST a %d\n", i.a.constant);
            return;
        }
        *(&s->x + fieldOffset(i.a.var)) = input;
        if(DEBUG_INST) {
            printf("inp %c=%d\n", i.a.var, input);
        }
    }
    
    if(i.type == ADD) {
        if(i.aType == CONST) {
            printf("Bad add with CONST a %d\n", i.a.constant);
            return;
        }
        int a = *(&s->x + fieldOffset(i.a.var));
        
        int b;
        if(i.bType == CONST) {
            b = i.b.constant;
        }else{
            b = *(&s->x + fieldOffset(i.b.var));
        }
        
        int result = a + b;
        if(DEBUG_INST) {
            if(i.bType == CONST) {
                printf("add %c=%d + %d => %d\n", i.a.var, a, b, result);
            }else{
                printf("add %c=%d + %c=%d => %d\n", i.a.var, a, i.b.var, b, result);
            }
        }
        *(&s->x + fieldOffset(i.a.var)) = result;
        return;
    }
    
    if(i.type == MUL) {
        if(i.aType == CONST) {
            printf("Bad mul with CONST a %d\n", i.a.constant);
            return;
        }
        int a = *(&s->x + fieldOffset(i.a.var));
        
        int b;
        if(i.bType == CONST) {
            b = i.b.constant;
        }else{
            b = *(&s->x + fieldOffset(i.b.var));
        }
        
        int result = a * b;
        if(DEBUG_INST) {
            if(i.bType == CONST) {
                printf("mul %c=%d * %d => %d\n", i.a.var, a, b, result);
            }else{
                printf("mul %c=%d * %c=%d => %d\n", i.a.var, a, i.b.var, b, result);
            }
        }
        *(&s->x + fieldOffset(i.a.var)) = result;
        return;
    }
    
    if(i.type == DIV) {
        if(i.aType == CONST) {
            printf("Bad div with CONST a %d\n", i.a.constant);
            return;
        }
        int a = *(&s->x + fieldOffset(i.a.var));
        
        int b;
        if(i.bType == CONST) {
            b = i.b.constant;
        }else{
            b = *(&s->x + fieldOffset(i.b.var));
        }
        
        int result = a / b;
        if(DEBUG_INST) {
            if(i.bType == CONST) {
                printf("div %c=%d / %d => %d\n", i.a.var, a, b, result);
            }else{
                printf("div %c=%d / %c=%d => %d\n", i.a.var, a, i.b.var, b, result);
            }
        }
        *(&s->x + fieldOffset(i.a.var)) = result;
        return;
    }
    
    if(i.type == MOD) {
        if(i.aType == CONST) {
            printf("Bad mod with CONST a %d\n", i.a.constant);
            return;
        }
        int a = *(&s->x + fieldOffset(i.a.var));
        
        int b;
        if(i.bType == CONST) {
            b = i.b.constant;
        }else{
            b = *(&s->x + fieldOffset(i.b.var));
        }
        
        int result = a % b;
        if(DEBUG_INST) {
            if(i.bType == CONST) {
                printf("mod %c=%d %% %d => %d\n", i.a.var, a, b, result);
            }else{
                printf("mod %c=%d %% %c=%d => %d\n", i.a.var, a, i.b.var, b, result);
            }
        }
        *(&s->x + fieldOffset(i.a.var)) = result;
        return;
    }
    
    if(i.type == EQL) {
        if(i.aType == CONST) {
            printf("Bad eql with CONST a %d\n", i.a.constant);
            return;
        }
        int a = *(&s->x + fieldOffset(i.a.var));
        
        int b;
        if(i.bType == CONST) {
            b = i.b.constant;
        }else{
            b = *(&s->x + fieldOffset(i.b.var));
        }
        
        int result = (a == b);
        if(DEBUG_INST) {
            if(i.bType == CONST) {
                printf("eql %c=%d == %d => %d\n", i.a.var, a, b, result);
            }else{
                printf("eql %c=%d == %c=%d => %d\n", i.a.var, a, i.b.var, b, result);
            }
        }
        *(&s->x + fieldOffset(i.a.var)) = result;
        return;
    }
}

struct digit {
    struct instruction **instructions;
    int count;
    struct state inputVars;
};

struct goodInputs {
    int inputs[2048];
    int count;
};

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 16;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	struct instruction *instructions = malloc(256 * sizeof(struct instruction));

	while(fgets(buffer, maxLineLength, file))
	{
		instructions[lineCount] = parseLine(buffer);
		lineCount++;
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	fclose(file);
    
    struct digit *digitInstructions = malloc(14 * sizeof(struct digit));
    int digitCount = -1;
    
    for(int i=0; i<lineCount; i++) {
        struct instruction *this = &instructions[i];
        if(this->type == INPUT) {
            digitCount++;
            digitInstructions[digitCount].count = 0;
            digitInstructions[digitCount].inputVars.x = 0;
            digitInstructions[digitCount].inputVars.y = 0;
            digitInstructions[digitCount].inputVars.z = 0;
            digitInstructions[digitCount].inputVars.w = 0;
            digitInstructions[digitCount].instructions = malloc(32 * sizeof(size_t));
        }
        
        struct digit *d = &digitInstructions[digitCount];
        d->instructions[d->count] = this;
        d->count++;
    }
    
    bool canChangeX[14] = { false };

    for(int digitIndex=0; digitIndex<14; digitIndex++) {
        struct digit *d = &digitInstructions[digitIndex];

        for(int i=d->count-1; i>=0; i--) {
            struct instruction *di = d->instructions[i];
            
            if(di->type == ADD && di->aType == VAR && di->a.var == 'x') {
                if(di->b.constant > 9 || di->b.constant < -24) {
                    //printf("Digit %d cannot change it's x\n", digitIndex+1);
                }else{
                    printf("Digit %d can change it's x\n", digitIndex+1);
                    canChangeX[digitIndex] = true;
                }
            }
            
            if(di->type == MUL) {
                if(di->bType == CONST && di->b.constant == 0) {
                    *(&d->inputVars.x + fieldOffset(di->a.var)) = 0;
                }
                continue;
            }
            
            if(di->aType == VAR) {
                *(&d->inputVars.x + fieldOffset(di->a.var)) = 1;
            }
            if(di->bType == VAR) {
                *(&d->inputVars.x + fieldOffset(di->b.var)) = 1;
            }
        }
        
        printf("Variables for digit %d\n", digitIndex + 1);
        printState(d->inputVars);
    }
   
    printf("\n");
   
    struct state state = { 0, 0, 0, 0 };
    char num[14] = { 1 };
    
    bool search(char (*num)[14], int digit, struct state start) {
        struct state s = start;
        if(canChangeX[digit]) {
            for(int w=1; w<10; w++) {
                struct digit *d = &digitInstructions[digit];
                s = start;
                (*num)[digit] = w;

                for(int i=0; i<d->count; i++) {
                    execute(*d->instructions[i], &s, w);
                }
                
                if(digit == 13) {
                    if(s.z < 10) {
                        printf("Result z: %d\n", s.z);
                        for(int d=0; d<14; d++) {
                            printf("%d", (*num)[d]);
                        }
                        printf("\n");
                    }
                    if(s.z == 0) {
                        return true;
                    }
                }
                
                if(s.x != 0) {
                    continue;
                }
            
                if(search(num, digit+1, s)) {
                    return true;
                }
            }
        
            return false;
        }
        
        for(int w=1; w<10; w++) {
            struct digit *d = &digitInstructions[digit];
            s = start;
            (*num)[digit] = w;

            for(int i=0; i<d->count; i++) {
                execute(*d->instructions[i], &s, w);
            }
            
            if(digit == 13) {
                if(s.z < 10) {
                    printf("Result z: %d\n", s.z);
                    for(int d=0; d<14; d++) {
                        printf("%d", (*num)[d]);
                    }
                    printf("\n");
                }
                return s.z == 0;
            }
        
            if(search(num, digit+1, s)) {
                return true;
            }
        }
        
        return false;
    }
    
    if(search(&num, 0, state)) {
        for(int d=0; d<14; d++) {
            printf("%d", num[d]);
        }
        printf("\n");
    }else{
        printf("Failed\n");
        for(int d=0; d<14; d++) {
            printf("%d", num[d]);
        }
        printf("\n");
    }
    
	return 0;
}
