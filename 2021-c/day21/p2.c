#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

#define SCORE (21)
// The index is the roll-sum outcome, zero-indexed
int universesPerOutcome[10] = {0, 0, 0, 1, 3, 6, 7, 6, 3, 1};

long long int player1wins = 0;
long long int player2wins = 0;

struct state {
    long long int universes;
    char roll1;
    char roll2;
    char player1pos;
    char player1score;
    char player2pos;
    char player2score;
    bool player1;
};

void simulate(int p1start, int p2start) {
    struct state stack[128] = { 0 };
    int stackPointer = 0;
    
    struct state current = {
        1, 0, 0, p1start, 0, p2start, 0, true
    };
    
    stack[stackPointer] = current;
    stackPointer++;

    while(stackPointer > 0) {
        while((current.player1 && current.roll1 < 10) ||
             (!current.player1 && current.roll2 < 10)) {
            int roll = current.player1 ? current.roll1 : current.roll2;
            int rollUs = universesPerOutcome[roll];
            if(rollUs == 0) {
                if(current.player1){
                    current.roll1++;
                }else{
                    current.roll2++;
                }
                continue;
            }
            
            if(current.player1){
                // So it's ready for when we come back
                current.roll1++;
            }else{
                current.roll2++;
            }

            stack[stackPointer] = current;
            stackPointer++;
            
            struct state new = {
                current.universes * rollUs, 0, 0,
                current.player1pos,
                current.player1score,
                current.player2pos,
                current.player2score,
                !current.player1
            };
                
            if(current.player1) {        
                new.player1pos = (new.player1pos + roll) % 10;
                new.player1score += new.player1pos + 1;
                if(new.player1score >= SCORE) {
                    player1wins += new.universes;
                    break;
                }
            }else{
                new.player2pos = (new.player2pos + roll) % 10;
                new.player2score += new.player2pos + 1;
                
                if(new.player2score >= SCORE) {
                    player2wins += new.universes;
                    break;
                }
            }
            
            current = new;
        }
        
        stackPointer--;
        current = stack[stackPointer];
    }
    
    printf("Player 1 wins: %lld\n", player1wins);
    printf("Player 2 wins: %lld\n", player2wins);
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 32;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	int player1pos = 0;
	int player2pos = 0;

    fgets(buffer, maxLineLength, file);
	lineCount++;
	
	strtok(buffer, ":");
	sscanf(strtok(NULL, ":"), "%d", &player1pos);
	player1pos--; // Zero index the pos
	
    fgets(buffer, maxLineLength, file);
	lineCount++;
	
	strtok(buffer, ":");
	sscanf(strtok(NULL, ":"), "%d", &player2pos);
	player2pos--; // Zero index the pos

	printf("Found %d lines in %s.\n", lineCount, filename);
	printf("Starting at %d & %d\n", player1pos, player2pos);

	fclose(file);
	
	simulate(player1pos, player2pos);

	return 0;
}
