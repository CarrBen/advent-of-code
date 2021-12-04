#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <limits.h>

#define PRINT_CARDS (false)

#define CARD_WIDTH (5)
#define CARD_HEIGHT (5)

struct cardResult {
	int winningNumber;
	int numbersDrawn;
	int sumOfUnmarked;
};

void markNumber(int calledNumber, int *cardNumbers, char *markedNumbers)
{
	for(int i=0; i<CARD_WIDTH; i++)
	{
		for(int j=0; j<CARD_HEIGHT; j++)
		{
			int index = i * CARD_HEIGHT + j;
			if(cardNumbers[index] == calledNumber)
			{
				markedNumbers[index] = 1;
				return;
			}
		}
	}
}

bool cardWon(char *markedNumbers)
{
	// Check rows
	for(int i=0; i<CARD_HEIGHT; i++)
	{
		int rowTotal = 0;
		for(int j=0; j<CARD_WIDTH; j++)
		{
			int index = i * CARD_WIDTH + j;
			if(markedNumbers[index] > 0)
			{
				rowTotal++;
			}
		}
		
		if(rowTotal == CARD_WIDTH)
		{
			return true;
		}
	}
	
	// Check cols
	for(int j=0; j<CARD_WIDTH; j++)
	{
		int colTotal = 0;
		for(int i=0; i<CARD_HEIGHT; i++)
		{
			int index = i * CARD_WIDTH + j;
			if(markedNumbers[index] > 0)
			{
				colTotal++;
			}
		}
		
		if(colTotal == CARD_HEIGHT)
		{
			return true;
		}
	}
	
	return false;
}

int sumUnmarked(int *cardNumbers, char *markedNumbers)
{
	int sum = 0;
	
	for(int i=0; i<CARD_HEIGHT; i++)
	{
		for(int j=0; j<CARD_WIDTH; j++)
		{
			int index = i * CARD_WIDTH + j;
			if(markedNumbers[index] == 0)
			{
				sum += cardNumbers[index];
			}
		}
	}
	
	return sum;
}

void printCardInt(int *cardNumbers)
{
	if(!PRINT_CARDS)
	{
		return;
	}
	
	for(int i=0; i<CARD_HEIGHT; i++)
	{
		for(int j=0; j<CARD_WIDTH; j++)
		{
			int index = i * CARD_WIDTH + j;
			printf("%d ", cardNumbers[index]);
		}
		printf("\n");
	}
	printf("\n");
}

void printCardChar(char *markedNumbers)
{
	if(!PRINT_CARDS)
	{
		return;
	}

	for(int i=0; i<CARD_HEIGHT; i++)
	{
		for(int j=0; j<CARD_WIDTH; j++)
		{
			int index = i * CARD_WIDTH + j;
			printf("%d ", markedNumbers[index]);
		}
		printf("\n");
	}
	printf("\n");
}

struct cardResult loadCard(int callCount, int *calls, FILE *file)
{
	int lineChunk = 128;
	char buffer[lineChunk];
	
	int *cardNumbers = malloc(CARD_WIDTH * CARD_HEIGHT * sizeof(int));
	int rowsLoaded = 0;
	
	while(fgets(buffer, lineChunk, file))
	{
		if(strlen(buffer) == 1)
		{
			// We've found an empty line between cards
			// So we're done loading this card
			break;
		}
		
		int colsLoaded = 0;
		char *token = strtok(buffer, " ");
		while(token)
		{
			sscanf(token, "%d", &cardNumbers[rowsLoaded * CARD_WIDTH + colsLoaded]);
			token = strtok(NULL, " ");
			colsLoaded++;
		}
		rowsLoaded++;
	}
	
	int lastDrawn = 0;
	int numbersDrawn = 0;
	char *markedNumbers = malloc(CARD_WIDTH * CARD_HEIGHT * sizeof(char));
	memset(markedNumbers, 0, CARD_WIDTH * CARD_HEIGHT * sizeof(char));
	
	printCardInt(cardNumbers);
	printCardChar(markedNumbers);
	
	for(int i=0; i<callCount; i++)
	{
		lastDrawn = calls[i];
		numbersDrawn++;
		markNumber(calls[i], cardNumbers, markedNumbers);
		
		printCardChar(markedNumbers);
		
		if(cardWon(markedNumbers))
		{
			break;
		}
	}
	
	int sumOfUnmarked = sumUnmarked(cardNumbers, markedNumbers);
	
	struct cardResult result = { lastDrawn, numbersDrawn, sumOfUnmarked };
	return result;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int lineChunk = 128;
	char buffer[lineChunk];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int callCount = 0;
	int callsBytes = 0;
	int cardCount = 0;
	bool cardMode = false;

	while(fgets(buffer, lineChunk, file))
	{
		if(cardMode)
		{
			// One for the \n
			if(strlen(buffer) == 1)
			{
				cardCount++;
			}
		}else{
			for(int i=0; i<lineChunk; i++)
			{
				callsBytes++;
				if(buffer[i] == '\0')
				{
					break;
				}
				
				if(buffer[i] == '\n')
				{
					// One more called number after the last comma
					callCount++;
					cardMode = true;
					break;
				}
				
				if(buffer[i] == ',')
				{
					callCount++;
				}
			}	
		}
	}

	printf("Found %d called numbers using %d bytes.\n", callCount, callsBytes);
	printf("Found %d bingo cards.\n", cardCount);
	
	int *calledNumbers = malloc(sizeof(int) * callCount);
	char *calledBuffer = malloc(callsBytes + 1);
	
	fseek(file, 0, SEEK_SET);
	fgets(calledBuffer, callsBytes + 1, file);
	int loadedCalls = 0;
	char* token = strtok(calledBuffer, ",");
	do
	{
		sscanf(token, "%d", &calledNumbers[loadedCalls]);
		loadedCalls++;
		token = strtok(NULL, ",");
	}
	while(token);
	
	printf("Loaded %d called numbers:\n", loadedCalls);
	for(int i=0; i<loadedCalls; i++)
	{
		printf("%d,", calledNumbers[i]);
	}
	printf("\n");
	
	// Skip newline so that next get will be the first row of the first card
	fgets(calledBuffer, callsBytes + 1, file);

	struct cardResult bingoResults[cardCount];
	for (int loadedCards=0; loadedCards<cardCount; loadedCards++)
	{
		bingoResults[loadedCards] = loadCard(loadedCalls, calledNumbers, file);
	}
	
	int fastestWin = INT_MAX;
	for (int i=0; i<cardCount; i++)
	{
		if(bingoResults[i].numbersDrawn > fastestWin)
		{
			continue;
		}
		fastestWin = bingoResults[i].numbersDrawn;
		
		printf("Won (or ended) after %d calls with %d. Sum: %d. Answer: %d\n",
			bingoResults[i].numbersDrawn,
			bingoResults[i].winningNumber,
			bingoResults[i].sumOfUnmarked,
			bingoResults[i].winningNumber * bingoResults[i].sumOfUnmarked
		);
	}
	
	fclose(file);

	return 0;
}
