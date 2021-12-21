#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int diceRolls = 0;
int nextRoll = 0;

int roll() {
    diceRolls++;
    
    int r = nextRoll + 1;
    
    nextRoll = (nextRoll + 1) % 100;
    
    return r;
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
	int player1score = 0;
	
	int player2pos = 0;
	int player2score = 0;

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

	fclose(file);

    while(player1score < 1000 && player2score < 1000) {
        for(int d=0; d<3; d++) {
            int r = roll();
            
            player1pos = (player1pos + r) % 10;
        }
        
        player1score += (player1pos + 1);
        
        if(player1score >= 1000) {
            break;
        }
        
        for(int d=0; d<3; d++) {
            int r = roll();
            
            player2pos = (player2pos + r) % 10;
        }
        
        player2score += (player2pos + 1);
    }
    
    printf("Player 1 %d * %d = %d\n", diceRolls, player1score, diceRolls * player1score);
    printf("Player 2 %d * %d = %d\n", diceRolls, player2score, diceRolls * player2score);

	return 0;
}
