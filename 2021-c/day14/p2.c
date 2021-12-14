#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <limits.h>

struct node {
	struct node *next;
	char value;
};

struct insert {
	struct insert *next;
	struct node *left;
	char value;
};

struct rule {
	char left;
	char right;
	char insert;
};

void printChain(struct node *chain) {
	struct node *n = chain;
	while(n) {
		printf("%c", n->value);
		n = n->next;
	}
	printf("\n");
}

long int cache[30][26*26][27] = { 0 };

void descend(char left, char right, struct rule rules[26*26], int ruleCount, long int counts[26], char step) {
	if(step > 40) {
		return;
	}
	
	if(step > 10) {
		long int *cachedCounts = cache[step-11][26 * (left - 'A') + (right - 'A')];
		if(cachedCounts[26] > 0) {
			long int sum = 0;
			for(int i=0; i<26; i++) {
				sum += cachedCounts[i];
				counts[i] += cachedCounts[i];
			}
			return;
		}
	}
	
	//printf("%d %c%c", step, left, right);
	
	struct rule *matchingRule = &rules[26 * (left - 'A') + (right - 'A')];
	
	if(matchingRule->insert == '\0') {
		return;
	}
	
	long int startCounts[26] = { 0 };
	for(int i=0; i<26; i++) {
		startCounts[i] = counts[i];
	}
	
	counts[matchingRule->insert - 'A']++;
	
	//printf(" -> %c\n", matchingRule->insert);
	
	descend(left, matchingRule->insert, rules, ruleCount, counts, step + 1);
	descend(matchingRule->insert, right, rules, ruleCount, counts, step + 1);
	
	// Populate the cache
	if(step <= 10) {
		return;
	}
	
	long int *cachedCounts = cache[step-11][26 * (left - 'A') + (right - 'A')];
	cachedCounts[26] = 1;
	for(int i=0; i<26; i++) {
		cachedCounts[i] = counts[i] - startCounts[i];
	}
	return;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 16;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int chainLength = 0;
	int ruleCount = 0;
	
	struct node *chain = malloc(sizeof(struct node));
	struct node *current = NULL;
	chain->next = NULL;

	while(fgets(buffer, maxLineLength, file))
	{
		if(buffer[0] == '\n' || buffer[0] == '\0') {
			break;
		}
		
		int i = 0;
		while(buffer[i] != '\0' && buffer[i] != '\n' && i < maxLineLength) {
			if(current == NULL) {
				current = chain;
			} else {
				current->next = malloc(sizeof(struct node));
				current = current->next;
			}
			
			current->value = buffer[i];
			chainLength++;
			
			i++;
		}
		
	}
	
	struct rule rules[26*26] = { 0 };
	
	while(fgets(buffer, maxLineLength, file))
	{
		if(buffer[0] == '\n' || buffer[0] == '\0') {
			continue;
		}
		
		char *left = strtok(buffer, " -> ");
		char *right = strtok(NULL, " -> ");
		
		struct rule *r = &rules[26 * (left[0] - 'A') + (left[1] - 'A')];
		r->left = left[0];
		r->right = left[1];
		r->insert = right[0];
		
		ruleCount++;
	}

	printf("Found %d chars in %s.\n", chainLength, filename);
	//printChain(chain);
	printf("Found %d rules in %s.\n", ruleCount, filename);
	
	long int counts[26] = { 0 };
	
	struct node *pointer = chain;
	counts[pointer->value - 'A']++;
	while(pointer && pointer->next) {
		counts[pointer->next->value - 'A']++;
		descend(pointer->value, pointer->next->value, rules, ruleCount, counts, 1);
		pointer = pointer->next;
	}
	
	long int leastCommon = LONG_MAX;
	long int mostCommon = 0;
	
	for(int i=0; i<26; i++) {
		if(counts[i] > 0 && counts[i] < leastCommon) {
			leastCommon = counts[i];
		}
		if(counts[i] > mostCommon) {
			mostCommon = counts[i];
		}
	}

	printf("%ld %ld %ld\n", leastCommon, mostCommon, mostCommon - leastCommon);
	
	fclose(file);

	return 0;
}
