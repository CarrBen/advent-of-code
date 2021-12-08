#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int charsToBitmap(char *chars) {
	int output = 0;
	for (int i=0; i<strlen(chars); i++) {
		if(chars[i] == ' ' || chars[i] == '\n') {
			continue;
		}
		int c = 1;
		c <<= chars[i] - 'a';
		output |= c;
	}
	return output;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 128;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	long int outputSum = 0;

	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;
		char *uniques = "";
		char *output = "";
		
		uniques = strtok(buffer, "|");
		output = strtok(NULL, "|");
		
		char *uniqueDigits[10];
		int digitsParsed = 0;
		
		char *token = strtok(uniques, " ");
		while(token){
			uniqueDigits[digitsParsed] = token;
			digitsParsed++;
			token = strtok(NULL, " ");
		}
		
		int one_bitmap, four_bitmap, seven_bitmap, eight_bitmap;
		
		// Find our known digits based on number of segments and unique digits
		for(int i=0; i<10; i++) {
			if (strlen(uniqueDigits[i]) == 2) {
				one_bitmap = charsToBitmap(uniqueDigits[i]);
			} else if (strlen(uniqueDigits[i]) == 3) {
				seven_bitmap = charsToBitmap(uniqueDigits[i]);
			} else if (strlen(uniqueDigits[i]) == 4) {
				four_bitmap = charsToBitmap(uniqueDigits[i]);
			} else if (strlen(uniqueDigits[i]) == 7) {
				eight_bitmap = charsToBitmap(uniqueDigits[i]);
			}
		}
		
		// Seven illuminates the one on the top and the two on the right. So remove the two on the right
		int seg_top_cent = one_bitmap ^ seven_bitmap;
		// Top left & mid from four without one
		int segs_tl_mid = four_bitmap ^ one_bitmap;
		
		int two_bitmap, three_bitmap, five_bitmap;
		
		for(int i=0; i<10; i++) {
			if (strlen(uniqueDigits[i]) == 5) {
				// Two, three or five
				int bitmap = charsToBitmap(uniqueDigits[i]);
				if ((bitmap & segs_tl_mid) == segs_tl_mid) {
					// Five has both TL & mid, others only have 1
					five_bitmap = bitmap;
				} else if ((bitmap & one_bitmap) == one_bitmap) {
					// Three has both on the right, others only have 1
					three_bitmap = bitmap;
				} else {
					two_bitmap = bitmap;
				}
			}
		}
		
		// Two doesn't have TL, but does have mid
		int seg_mid = two_bitmap & segs_tl_mid;
		// TL & Mid, without the mid
		int seg_top_left = segs_tl_mid ^ seg_mid;
		// Top right is shared by one and two
		int seg_top_right = one_bitmap & two_bitmap;
		// Bottom right is shared by one and five
		int seg_bot_right = one_bitmap & five_bitmap;
		// Five with the other four segements removed isolates bottom cent
		int seg_bot_cent = five_bitmap ^ seg_top_cent ^ seg_top_left ^ seg_mid ^ seg_bot_right;
		// Two with the other four segements removed isolates bottom left
		int seg_bot_left = two_bitmap ^ seg_top_cent ^ seg_top_right^ seg_mid ^ seg_bot_cent;
		
		// Construct digits from segments;
		int digits[10];
		digits[0] = seg_top_cent | seg_top_right | seg_bot_right | seg_bot_cent | seg_bot_left | seg_top_left;
		digits[1] = one_bitmap;
		digits[2] = two_bitmap;
		digits[3] = three_bitmap;
		digits[4] = four_bitmap;
		digits[5] = five_bitmap;
		digits[6] = eight_bitmap ^ seg_top_right;
		digits[7] = seven_bitmap;
		digits[8] = eight_bitmap;
		digits[9] = eight_bitmap ^ seg_bot_left;
		
// 		printf("%d\n", seg_top_cent);
// 		printf("%d\n", seg_top_left);
// 		printf("%d\n", seg_top_right);
// 		printf("%d\n", seg_mid);
// 		printf("%d\n", seg_bot_left);
// 		printf("%d\n", seg_bot_right);
// 		printf("%d\n", seg_bot_cent);
// 		
// 		printf("---\n");
// 		
// 		for(int i=0; i<10; i++) {
// 			printf("%d\n", digits[i]);
// 		}
// 		
// 		printf("---\n");
		
		int currentSum = 0;
		
		token = strtok(output, " ");
		while(token){
			int bitmap = charsToBitmap(token);
			currentSum *= 10;
			
			for(int i=0; i<10; i++) {
				if (digits[i] == bitmap) {
					currentSum += i;
				}
			}
						
			token = strtok(NULL, " ");
		}
		
		outputSum += currentSum;
		printf("%d\n", currentSum);
	}

	printf("Found %d lines in %s.\n", lineCount, filename);

	printf("Output sum was %ld\n", outputSum);
	
	fclose(file);

	return 0;
}
