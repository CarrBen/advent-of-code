#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <math.h>

#define POD_COUNT (16)
#define GRID_HEIGHT (7)

long long int moves = 0;
int mostFinished = 0;

struct moveResult {
    char grid[GRID_HEIGHT][16];
    int cost;
    bool finished;
};

struct pos {
    int x;
    int y;
};

struct state {
    struct pos neverMoved[POD_COUNT];
    struct pos moved[POD_COUNT];
    struct pos finishedMoving[POD_COUNT];
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

void printGrid(char grid[GRID_HEIGHT][16]);
void printState(struct state state) {
    printf("Never moved: ");
    for(int index=0; index<POD_COUNT; index++) {
        if(state.neverMoved[index].x == 0) continue;
        printf("%d,%d ", state.neverMoved[index].x, state.neverMoved[index].y);
    }
    printf("\n");
    printf("Moved: ");
    for(int index=0; index<POD_COUNT; index++) {
        if(state.moved[index].x == 0) continue;
        printf("%d,%d ", state.moved[index].x, state.moved[index].y);
    }
    printf("\n");
    printf("Finished moving: ");
    for(int index=0; index<POD_COUNT; index++) {
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

bool canMove(char type, int x, int y, char grid[GRID_HEIGHT][16]) {
    if(!isPod(type)) return false;
    // In side room and not blocked
    if(y >= 2) {
        bool allFree = true;
        for(int i=y-1; i>=1; i--) {
            allFree = allFree && grid[i][x] == '.';
        }
        if(allFree) return true;
    }
    // In hallway and target side room has space
    if(y == 1) {
        int tsrx = targetSideRoom(type);
        bool hasSpaceAndReady = true;
        bool checkingEmpty = true;
        for(int cy=2; cy<=GRID_HEIGHT-2; cy++) {
            if(checkingEmpty) {
                if(grid[cy][tsrx] != '.') {
                    checkingEmpty = false;
                }else{
                    continue;
                }
            }
            if(!checkingEmpty) {
                if(grid[cy][tsrx] != type) {
                    hasSpaceAndReady = false;
                    break;
                }
            }
        }
        return hasSpaceAndReady;
    }
    return false;
}

bool isBlocking(char type, int x, int y, char grid[GRID_HEIGHT][16]) {
    if(isInSideRoom(type, x, y)) {
        bool anyMismatched = false;
        
        for(int i=y+1; i<=(GRID_HEIGHT-2); i++) {
            anyMismatched = anyMismatched || (grid[i][x] != '.' && grid[i][x] != type);
        }
        
        return anyMismatched;
    }
    
    return false;
}

bool shouldMove(char type, int x, int y, char grid[GRID_HEIGHT][16]) {
    if(!isPod(type)) return false;
    if(isBlocking(type, x, y, grid)) return true;
    
    if(y == 1) {
        int tsrx = targetSideRoom(type);
        if(grid[2][tsrx] != '.') {
            return false;
        }
        for(int cy=3; cy<=GRID_HEIGHT-2; cy++) {
            if(grid[cy][tsrx] != '.' && grid[cy][tsrx] != type) {
                return false;
            }
        }
    }
    
    return true;
}

struct moveResult makeMove(char grid[GRID_HEIGHT][16], struct state state, int cost, int bestFinishedCost) {
    struct moveResult bestResult = { 0 };
    bestResult.cost = bestFinishedCost;
    bool moved = false;
    
    for(int i=0; i<(2*POD_COUNT); i++) {
        struct pos pod;
        if(i<POD_COUNT) {
            pod = state.moved[i];
        }else{
            pod = state.neverMoved[i-POD_COUNT];
        }
        
        if(pod.x == 0 && pod.y == 0) continue;
        
        int x = pod.x;
        int y = pod.y;
        
        if(isInSideRoom(grid[y][x], x, y) && !isBlocking(grid[y][x], x, y, grid)) {
            if(i<POD_COUNT) {
                state.moved[i].x = 0;
                state.moved[i].y = 0;
            }else{
                state.neverMoved[i-POD_COUNT].x = 0;
                state.neverMoved[i-POD_COUNT].y = 0;
            }
            
            int index = 0;
            while(state.finishedMoving[index].x != 0) {
                index++;
            }
            state.finishedMoving[index].x = x;
            state.finishedMoving[index].y = y;
            
            /*
            struct moveResult result = makeMove(grid, state, cost, bestResult.cost);
        
            if(result.finished && result.cost < bestResult.cost) {
                bestResult = result;                
            }
            */
            
            continue;
        }

        if(cost == 0) {
            printf("moves: %lld\n", moves);
            moves = 0;
            printf("y: %d x: %d\n", y, x);
        }
        char c = grid[y][x];
        //printf("%c @ %d,%d = %d %d\n", c, x, y, shouldMove(c, x, y, grid), canMove(c, x, y, grid));
        if(shouldMove(c, x, y, grid) && canMove(c, x, y, grid)) {
            bool couldMoveToTarget = false;
            for(int t=0; t<(GRID_HEIGHT - 3 + 7); t++) {               
                int tx = 0;
                int ty = 0;
                bool movingToTarget = false;
                
                if(couldMoveToTarget) break;
                
                if(t <= (GRID_HEIGHT - 4)) {
                    tx = targetSideRoom(c);
                    ty = GRID_HEIGHT - 2 - t;
                    movingToTarget = true;
                }else{
                    // i<8 means in hallway, so only need to check final destination
                    if(i < POD_COUNT) break;         
                    ty = 1;
                    tx = hallway[t-(GRID_HEIGHT - 3)];
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
                if(ty >= 2 && ty<= GRID_HEIGHT-3 && tx == targetSideRoom(c) && grid[ty+1][tx] == '.') {
                    //printf("Skip to back\n");
                    continue;
                }
                
                char nextGrid[GRID_HEIGHT][16] = { 0 };
                struct state nextState = { 0 };
                memcpy(&nextGrid, grid, GRID_HEIGHT * 16 * sizeof(char));
                memcpy(&nextState, &state, sizeof(struct state));
                if(i<POD_COUNT) {
                    nextState.moved[i].x = 0;
                    nextState.moved[i].y = 0;
                    int index = 0;
                    while(nextState.finishedMoving[index].x != 0) {
                        index++;
                    }
                    nextState.finishedMoving[index].x = tx;
                    nextState.finishedMoving[index].y = ty;
                }else{
                    nextState.neverMoved[i-POD_COUNT].x = 0;
                    nextState.neverMoved[i-POD_COUNT].y = 0;
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
                
                //if(false && grid[2][5] == '.' && c == 'B' && nextGrid[2][5] == 'B') {
                
                if(cost + tc > bestResult.cost) continue;
                if(isBlocking(c, tx, ty, nextGrid)) continue;
                
                if(false) {
                    printf("\nWut\n");
                    printf("Moved to %d,%d\n", tx, ty);
                    printGrid(grid);
                    printState(state);
                    printGrid(nextGrid);
                    printState(nextState);
                }
                
                if(movingToTarget) {
                    couldMoveToTarget = true;
                }
                
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
        //printGrid(grid);
        //printState(state);
        memcpy(&bestResult.grid, grid, GRID_HEIGHT * 16 * sizeof(char));
        bestResult.cost = cost;
        
        int finCount = 0;
        for(int i=0; i<POD_COUNT; i++) {
            if(state.finishedMoving[i].x != 0) {
                finCount++;
            }
        }
        
        if(finCount > mostFinished) {
            mostFinished = finCount;
            printf("Most finished %d\n", finCount);
            printGrid(grid);
            printState(state);
        }
        
        bool fin = true;
        
        for(int y=1; y<(GRID_HEIGHT-1); y++) {
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

void printGrid(char grid[GRID_HEIGHT][16]) {
    for(int y=0; y<GRID_HEIGHT; y++) {
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
	
	char grid[GRID_HEIGHT][16] = { { ' ' } };

	while(fgets(buffer, maxLineLength, file))
	{
	    memcpy(grid[lineCount], buffer, strlen(buffer));
		lineCount++;
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	fclose(file);
	
	struct state state = { 0 };
	
	for(int y=0; y<GRID_HEIGHT; y++) {
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
