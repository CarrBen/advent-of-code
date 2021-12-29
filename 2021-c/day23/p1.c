#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <math.h>

long long int moves = 0;

struct moveResult {
    char grid[5][16];
    int cost;
    bool finished;
};

struct pos {
    int x;
    int y;
};

struct state {
    struct pos neverMoved[8];
    struct pos moved[8];
    struct pos finishedMoving[8];
};

int energyPerMove(char type) {
    switch(type) {
        case 'A':
            return 1;
            break;
        case 'B':
            return 10;
            break;
        case 'C':
            return 100;
            break;
        case 'D':
            return 1000;
            break;
        default:
            return -100000;
    };
}

int targetSideRoom(char type) {
    switch(type) {
        case 'A':
            return 3;
            break;
        case 'B':
            return 5;
            break;
        case 'C':
            return 7;
            break;
        case 'D':
            return 9;
            break;
        default:
            return 100;
    };    
}

int hallway[7] = { 1, 2, 4, 6, 8, 10, 11 };

void printGrid(char grid[5][16]);
void printState(struct state state) {
    printf("Never moved: ");
    for(int index=0; index<8; index++) {
        if(state.neverMoved[index].x == 0) continue;
        printf("%d,%d ", state.neverMoved[index].x, state.neverMoved[index].y);
    }
    printf("\n");
    printf("Moved: ");
    for(int index=0; index<8; index++) {
        if(state.moved[index].x == 0) continue;
        printf("%d,%d ", state.moved[index].x, state.moved[index].y);
    }
    printf("\n");
    printf("Finished moving: ");
    for(int index=0; index<8; index++) {
        if(state.finishedMoving[index].x == 0) continue;
        printf("%d,%d ", state.finishedMoving[index].x, state.finishedMoving[index].y);
    }
    printf("\n");
    printf("\n");
}

bool isPod(char type) {
    return type >= 'A' && type <= 'D';
}

bool isEmpty(char type) {
    return type == '.';
}

bool isInSideRoom(char type, int x, int y) {
    return x == targetSideRoom(type) && y >= 2;
}

bool canMove(char type, int x, int y, char grid[5][16]) {
    if(!isPod(type)) return false;
    // In side room and not blocked
    if(y >= 2 && grid[y-1][x] == '.') {
        return true;
    }
    // In hallway and target side room has space
    if(y == 1) {
        int tsrx = targetSideRoom(type);
        if(grid[2][tsrx] == '.' && grid[3][tsrx] == type) {
            return true;
        }
        if(grid[2][tsrx] == '.' && grid[3][tsrx] == '.') {
            return true;
        }
    }
    return false;
}

bool shouldMove(char type, int x, int y, char grid[5][16]) {
    if(!isPod(type)) return false;
    if(isInSideRoom(type, x, y)) {
        if(y == 2 && grid[3][x] != type) {
            return true;
        }
        // Not consider them being in the space next to the hall with the other space empty
        return false;
    }
    
    if(y == 1) {
        int tsrx = targetSideRoom(type);
        if(grid[2][tsrx] != '.') {
            return false;
        }
        if(grid[3][tsrx] != '.' && grid[3][tsrx] != type) {
            return false;
        }
    }
    
    return true;
}

struct moveResult makeMove(char grid[5][16], struct state state, int cost, int bestFinishedCost) {
    struct moveResult bestResult = { 0 };
    bestResult.cost = bestFinishedCost;
    bool moved = false;
    
    for(int i=0; i<16; i++) {
        struct pos pod;
        if(i<8) {
            pod = state.moved[i];
        }else{
            pod = state.neverMoved[i-8];
        }
        
        if(pod.x == 0 && pod.y == 0) continue;
        
        int x = pod.x;
        int y = pod.y;

        if(cost == 0) {
            printf("moves: %lld\n", moves);
            moves = 0;
            printf("y: %d x: %d\n", y, x);
        }
        char c = grid[y][x];
        //printf("%c @ %d,%d = %d %d\n", c, x, y, shouldMove(c, x, y, grid), canMove(c, x, y, grid));
        if(shouldMove(c, x, y, grid) && canMove(c, x, y, grid)) {
            for(int t=0; t<9; t++) {               
                int tx = 0;
                int ty = 0;
                
                if(t <= 1) {
                    tx = targetSideRoom(c);
                    ty = 3 - t;
                }else{
                    // i<8 mesnd in hallway, so only need to check final destination
                    if(i < 8) break;         
                    ty = 1;
                    tx = hallway[t-2];
                }

                if(!isEmpty(grid[ty][tx])) continue;
                if(ty == 1 && (tx == 3 || tx == 5 || tx == 7 || tx == 9)) continue;
                if(ty >= 2 && tx != targetSideRoom(c)) continue;
                
                bool blocked = false;
                if(y >= 2) {
                    for(int cy=y-1; cy>=1; cy--) {
                        if(!isEmpty(grid[cy][x])) {
                            blocked = true;
                        }
                    }
                }
                
                if(blocked) continue;
                
                if(x > tx) {
                    for(int cx=tx; cx<=x; cx++) {
                        if(cx == x && y == 1) continue;
                        if(!isEmpty(grid[1][cx])) {
                            blocked = true;
                        }
                    }
                }else{
                    for(int cx=x; cx<=tx; cx++) {
                        if(cx == x && y == 1) continue;
                        if(!isEmpty(grid[1][cx])) {
                            blocked = true;
                        }
                    }
                }
                
                if(blocked) continue;
                
                if(ty >= 2) {
                    for(int cy=ty-1; cy>=1; cy--) {
                        if(!isEmpty(grid[cy][tx])) {
                            blocked = true;
                        }
                    }
                }
                
                if(blocked) continue;
                
                // Move to back of room if possible
                if(ty == 2 && tx == targetSideRoom(c) && grid[ty+1][tx] == '.') {
                    continue;
                }
                
                char nextGrid[5][16] = { 0 };
                struct state nextState = { 0 };
                memcpy(&nextGrid, grid, 5 * 16 * sizeof(char));
                memcpy(&nextState, &state, sizeof(struct state));
                if(i<8) {
                    nextState.moved[i].x = 0;
                    nextState.moved[i].y = 0;
                    int index = 0;
                    while(nextState.finishedMoving[index].x != 0) {
                        index++;
                    }
                    nextState.finishedMoving[index].x = tx;
                    nextState.finishedMoving[index].y = ty;
                }else{
                    nextState.neverMoved[i-8].x = 0;
                    nextState.neverMoved[i-8].y = 0;
                    int index = 0;
                    while(nextState.moved[index].x != 0) {
                        index++;
                    }
                    nextState.moved[index].x = tx;
                    nextState.moved[index].y = ty;
                }
                nextGrid[ty][tx] = grid[y][x];
                nextGrid[y][x] = '.';
                int tc = abs(tx - x) + (y - 1 + ty - 1);
                tc *= energyPerMove(c);
                moved = true;
                moves++;
                
                if(cost + tc > bestResult.cost) continue;
                
                //printf("Try move %c %d,%d to %d,%d cost %d\n", c, x, y, tx, ty, cost + tc);
                //printState(nextState);
                struct moveResult result = makeMove(nextGrid, nextState, cost + tc, bestResult.cost);
        
                if(result.finished) {
                    //printf("%d,%d to %d,%d cost %d\n", x, y, tx, ty, result.cost);
                    //printGrid(grid);
                }
        
                if(result.finished && result.cost < bestResult.cost) {
                    bestResult = result;
                    
                    //printGrid(result.grid);
                    //printf("%d\n", result.cost);
                    
                }
            }
        }
    }
    
    if(!moved) {
        //printf("Checking finished\n");
        //printState(state);
        memcpy(&bestResult.grid, grid, 5 * 16 * sizeof(char));
        bestResult.cost = cost;
        
        bool fin = true;
        
        for(int y=1; y<4; y++) {
            for(int x=1; x<15; x++) {
                if(!isPod(grid[y][x])) continue;
                
                fin = isInSideRoom(grid[y][x], x, y) && fin;
                if(!fin) break;
            }
            if(!fin) break;
        }
        
        bestResult.finished = fin;
        
        if(fin) {
            //printf("Ye %d\n", bestResult.cost);
        }else{
            if(false) {
                printGrid(bestResult.grid);
                printf("%d\n", bestResult.cost);    
            }
        }
    }
    
    return bestResult;
}

void printGrid(char grid[5][16]) {
    for(int y=0; y<5; y++) {
        printf("%s", grid[y]);
        if(grid[y][strlen(grid[y])-1] != '\n') {
            printf("\n");
        }
    }
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 16;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	char grid[5][16] = { { ' ' } };

	while(fgets(buffer, maxLineLength, file))
	{
	    memcpy(grid[lineCount], buffer, strlen(buffer));
		lineCount++;
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	fclose(file);
	
	struct state state = { 0 };
	
	for(int y=0; y<5; y++) {
	    for(int x=0; x<13; x++) {
	        if(grid[y][x] >= 'A' && grid[y][x] <= 'D') {
	            int index = 0;
	            while(state.neverMoved[index].x != 0) {
	                index++;
	            }
	            state.neverMoved[index].x = x;
	            state.neverMoved[index].y = y;
	        }
	    }
	}

    struct moveResult result = makeMove(grid, state, 0, 9999999);
    
    printf("Cost was %d\n", result.cost);
    printGrid(result.grid);

	return 0;
}
