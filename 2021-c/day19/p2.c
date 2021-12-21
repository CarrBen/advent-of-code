#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <limits.h>
#include <math.h>
#include <stdbool.h>

struct beacon {
    short x;
    short y;
    short z;
    struct scanner *ref;
};

struct rotation {
    short x;
    short y;
    short z;
};

struct scannerLink {
    struct scanner *scanner;
    struct beacon position;
    int r;
    bool inverse;
};

struct scanner {
    int id;
    int beaconCount;
    struct beacon beacons[64];
    struct scannerLink *links;
    int linkCount;
};

struct region {
    int xMax;
    int xMin;
    int yMax;
    int yMin;
    int zMax;
    int zMin;
    struct rotation rot;
    int matchCount;
};

struct rotation uniqueRotations[24] = {
    // Forward stays the same
    { 0, 0, 0 },
    { 90, 0, 0 },
    { 180, 0, 0 },
    { 270, 0, 0 },
    
    // Rotate forward around z +90
    { 0, 0, 90 },
    { 0, 90, 90 },
    { 0, 180, 90 },
    { 0, 270, 90 },
    
    // Rotate forward around z +180
    { 0, 0, 180 },
    { 90, 0, 180 },
    { 180, 0, 180 },
    { 270, 0, 180 },
    
    // Rotate forward around z +270
    { 0, 0, 270 },
    { 0, 90, 270 },
    { 0, 180, 270 },
    { 0, 270, 270 },
    
    // Rotate forward around y +90
    { 0, 90, 0 },
    { 90, 0, 270 },
    { 180, 270, 0 }, //Should it be -90?
    { 270, 0, 90 },
    
    // Rotate forward around y -90
    { 0, 270, 0 },
    { 90, 0, 90 },
    { 180, 270, 0 },
    { 270, 0, 270 },
};

struct rotation inverseUniqueRotations[24] = {
    // Forward stays the same
    { 0, 0, 0 },
    { -90, 0, 0 },
    { -180, 0, 0 },
    { -270, 0, 0 },
    
    // Rotate forward around z +90
    { 0, 0, -90 },
    { 90, 0, -90 },
    { 0, 180, 90 },
    { -90, 0, -90 },
    
    // Rotate forward around z +180
    { 0, 0, 180 },
    { 90, 0, 180 },
    { 0, 180, 0 },
    { -90, 0, 180 },
    
    // Rotate forward around z +270
    { 0, 0, -270 },
    { -90, 0, 90 },
    { 180, 0, -270 },
    { 90, 0, 90 },
    
    // Rotate forward around y +90
    { 0, -90, 0 },
    { 0, 90, 90 },
    { 0, -90, 180 },
    { 0, 90, -90 },
    
    // Rotate forward around y -90
    { 0, -270, 0 },
    { 0, -90, -90 },
    { 0, 270, 180 },
    { 0, -90, 90 },
};

int beaconDistance(const struct beacon a, const struct beacon b) {
    int dx = a.x - b.x;
    int dy = a.y - b.y;
    int dz = a.z - b.z;
    return (dx * dx) + (dy * dy) + (dz * dz);
}

// Rotate the position of the beacon around the origin by some multiple of 90 degrees for each axis
struct beacon rotate(const struct beacon *b, short x, short y, short z) {
    struct beacon out = { b->x, b->y, b->z };
    out.ref = b->ref;
    
    float cx = cos(x / 180.0f * M_PI);
    float sx = sin(x / 180.0f * M_PI);
    
    float cy = cos(y / 180.0f * M_PI);
    float sy = sin(y / 180.0f * M_PI);
    
    float cz = cos(z / 180.0f * M_PI);
    float sz = sin(z / 180.0f * M_PI);
    
    out.x = round(cz*cy*b->x + (cz*sy*sx-sz*cx)*b->y + (cz*sy*cx+sz*sx)*b->z);
    out.y = round(sz*cy*b->x + (sz*sy*sx+cz*cx)*b->y + (sz*sy*cx-cz*sx)*b->z);
    out.z = round(b->x*-sy + cy*sx*b->y + cy*cx*b->z);
    
    return out;
}

bool inRegion(const struct beacon b, const struct region r) {
    if(b.x >= r.xMin && b.x <= r.xMax &&
        b.y >= r.yMin && b.y <= r.yMax &&
        b.z >= r.zMin && b.z <= r.zMax) {
        return true;
    }
    return false;
}

struct beacon add(const struct beacon b, const struct beacon o) {
    struct beacon out = {
        b.x + o.x,
        b.y + o.y,
        b.z + o.z
    };
    
    return out;
}

struct beacon rotateByStruct(const struct beacon b, const struct rotation r) {
    return rotate(&b, r.x, r.y, r.z);
}

struct region* overlap(struct scanner *a, struct scanner *b) {
    struct region *out = malloc(sizeof(struct region));
    out->matchCount = 0;
    
    for(int ba=0; ba<a->beaconCount; ba++) {
        const struct beacon beaconA = a->beacons[ba];
        
        if(out->matchCount > 0) {
            break;
        }
        
        for(int bb=0; bb<b->beaconCount; bb++) {
            const struct beacon beaconB = b->beacons[bb];
            
            if(out->matchCount > 0) {
                break;
            }
        
            for(int rb=0; rb<24; rb++) {
                int matches = 0;
                
                const struct beacon beaconRB = rotateByStruct(beaconB, uniqueRotations[rb]);
                const struct beacon maybeBinA = { 
                    beaconA.x - beaconRB.x, beaconA.y - beaconRB.y, beaconA.z - beaconRB.z };
                
                struct region shared = { 0 };
                shared.rot = uniqueRotations[rb];
                /*
                if(a.id == 15 && maybeBinA.x == 64 && maybeBinA.y == -1159 && maybeBinA.z == 12) {
                    printf("A %d %d,%d,%d\n", ba, beaconA.x, beaconA.y, beaconA.z);
                    printf("B %d %d,%d,%d\n", bb, beaconB.x, beaconB.y, beaconB.z);
                    printf("R %d %d,%d,%d\n", bb, beaconRB.x, beaconRB.y, beaconRB.z);
                }
                if(a.id == 25 && ba == 0 && bb == 1) {
                    printf("A %d %d,%d,%d\n", ba, beaconA.x, beaconA.y, beaconA.z);
                    printf("B %d %d,%d,%d\n", bb, beaconB.x, beaconB.y, beaconB.z);
                    printf("R %d %d,%d,%d\n", bb, beaconRB.x, beaconRB.y, beaconRB.z);
                    printf("Candidate pos %d,%d,%d\n", maybeBinA.x, maybeBinA.y, maybeBinA.z);
                }
                */
                if(maybeBinA.x > 2000 || maybeBinA.x < -2000 || maybeBinA.y > 2000 || maybeBinA.y < -2000 || maybeBinA.z > 2000 || maybeBinA.z < -2000){
                    printf("Scanners %d and %d too far apart\n", a->id, b->id);
                    printf("Candidate pos %d,%d,%d\n", maybeBinA.x, maybeBinA.y, maybeBinA.z);
                    printf("This shouldn't happen...");
                    continue;
                }
                
                // Determine region that would overlap
                if(maybeBinA.x > 0) {
                    shared.xMax = 1000;
                    shared.xMin = maybeBinA.x - 1000;
                }else{
                    shared.xMax = maybeBinA.x + 1000;
                    shared.xMin = -1000;
                }
                
                if(maybeBinA.y > 0) {
                    shared.yMax = 1000;
                    shared.yMin = maybeBinA.y - 1000;
                }else{
                    shared.yMax = maybeBinA.y + 1000;
                    shared.yMin = -1000;
                }
                
                if(maybeBinA.z > 0) {
                    shared.zMax = 1000;
                    shared.zMin = maybeBinA.z - 1000;
                }else{
                    shared.zMax = maybeBinA.z + 1000;
                    shared.zMin = -1000;
                }
                               
                int bInRegionA = 0;
                for(int i=0; i<a->beaconCount; i++) {
                    const struct beacon bir = a->beacons[i];
                    if(inRegion(bir, shared)) {
                        bInRegionA++;
                    }
                }
                
                int bInRegionB = 0;
                for(int i=0; i<b->beaconCount; i++) {
                    struct beacon bir = rotateByStruct(b->beacons[i], shared.rot);
                    
                    bir.x += maybeBinA.x;
                    bir.y += maybeBinA.y;
                    bir.z += maybeBinA.z;
                    
                    if(inRegion(bir, shared)) {
                        bInRegionB++;
                    }
                }
                
                //printf("%d/%d beacons in shared region\n", bInRegionA, bInRegionB);
                if(bInRegionA < 12 || bInRegionB < 12 || bInRegionA != bInRegionB) {
                    continue;
                }
                if(false) {
                    printf("%d/%d beacons in shared region\n", bInRegionA, bInRegionB);
                    printf("Shared region is %d,%d,%d to %d,%d,%d\n",
                        shared.xMin, shared.yMin, shared.zMin,
                        shared.xMax, shared.yMax, shared.zMax);
                }
                
                for(int i=0; i<a->beaconCount; i++) {
                    const struct beacon A = a->beacons[i];
                    if(!inRegion(A, shared)) {
                        continue;
                    }
                    
                    for(int j=0; j<b->beaconCount; j++) {
                        struct beacon B = rotateByStruct(b->beacons[j], shared.rot);
                        
                        B.x += maybeBinA.x;
                        B.y += maybeBinA.y;
                        B.z += maybeBinA.z;
                        
                        if(!inRegion(B, shared)) {
                            continue;
                        }
                        
                        if(B.x == A.x && B.y == A.y && B.z == A.z) {
                            //printf("%d %d\n", i, j);
                            //printf("%d,%d,%d\n", b.beacons[j].x, b.beacons[j].y, b.beacons[j].z);
                            //printf("%d,%d,%d\n", B.x, B.y, B.z);
                            matches++;
                        }
                    }
                }
                
                if(matches >= 12) {
                    //printf("Found %d matches\n", matches);
                    //printf("Candidate pos %d,%d,%d\n", maybeBinA.x, maybeBinA.y, maybeBinA.z);
                    
                    const struct beacon maybeAinB = { 
                        beaconRB.x - beaconA.x, beaconRB.y - beaconA.y, beaconRB.z - beaconA.z };
                    //printf("Candidate pos %d,%d,%d\n", maybeAinB.x, maybeAinB.y, maybeAinB.z);
                    //printf("rb %d\n", rb);
                    
                    out->matchCount = matches;
                    out->xMin = shared.xMin;   
                    out->xMax = shared.xMax;   
                    out->yMin = shared.yMin;   
                    out->yMax = shared.yMax;   
                    out->zMin = shared.zMin;   
                    out->zMax = shared.zMax;
                    out->rot = shared.rot;
                    
                    struct scannerLink linkToB = { b, maybeBinA, rb, false };
                    a->links[a->linkCount] = linkToB;
                    a->linkCount++;
                    
                    struct scannerLink linkToA = { a, maybeAinB, rb, true };
                    b->links[b->linkCount] = linkToA;
                    b->linkCount++; 
                    
                    break;
                }
            }
        }
    }
    
    return out;
}

int search(struct scanner *ref, struct scannerLink **path, int pathCount) {
    for(int l=0; l<ref->linkCount; l++) {
        struct scannerLink link = ref->links[l];
        if(link.scanner->id == 0) {
            path[pathCount] = &ref->links[l];
            return pathCount + 1;
        }
    }
    
    for(int l=0; l<ref->linkCount; l++) {
        struct scannerLink link = ref->links[l];
        path[pathCount] = &ref->links[l];
        
        bool inPath = false;
        for(int p=0; p<pathCount; p++) {
            if(path[p] == &ref->links[l]) {
                inPath = true;
            }
        }
        if(inPath) {
            continue;
        }
        
        int result = search(link.scanner, path, pathCount+1);
        if(result) {
            return result;
        }
    }
    
    return 0;
}

struct beacon transformToZero(struct beacon b) {
    if(b.ref == NULL || b.ref->id == 0) {
        return b;
    }
    
    struct scannerLink **path = malloc(128 * sizeof(size_t));
    
    int pathCount = search(b.ref, path, 0);
    printf("Path from %d to 0 has length %d\n", b.ref->id, pathCount);
    
    if(pathCount == 0) {
        struct beacon empty = { 0, 0, 0, 0 };
        return empty;
    }

    printf("Pos start %d,%d,%d scanner %d\n", b.x, b.y, b.z, b.ref->id);
    struct beacon pos = b;
    for(int p=0; p<pathCount; p++) {
        struct scannerLink link = *path[p];
        
        if(link.inverse) {
            pos = rotateByStruct(pos, uniqueRotations[link.r]);
            
            pos.x -= link.position.x;
            pos.y -= link.position.y;
            pos.z -= link.position.z;
        }else{
            pos.x -= link.position.x;
            pos.y -= link.position.y;
            pos.z -= link.position.z;
        
            pos = rotateByStruct(pos, inverseUniqueRotations[link.r]);
        }     

        pos.ref = link.scanner;
        
        printf("Pos step %d,%d,%d scanner %d invert %d\n", pos.x, pos.y, pos.z, link.scanner->id, link.inverse);
    }
        
    return pos;
}

int scannerToZero(struct scanner *s) {
    if(s->id == 0) {
        return 0;
    }
    
    for(int l=0; l<s->linkCount; s++) {
        struct scannerLink link = s->links[l];
        int result = scannerToZero(link.scanner);
        if(result >= 0) {
            printf("%d,", result);
            return link.scanner->id;
        }
    }
    
    return -1;
}

int beaconCmpX (const void * a, const void * b) {
    struct beacon ba = *(struct beacon*)a;
    struct beacon bb = *(struct beacon*)b;
    return ( ba.x - bb.x );
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 32;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	int scannerCount = 0;
	int allBeaconsCount = 0;
	
	struct scanner *scanners = malloc(32 * sizeof(struct scanner));
	struct scanner *s = NULL;

	while(fgets(buffer, maxLineLength, file))
	{
		lineCount++;
	    if(buffer[0] == '\n') {
	        continue;
	    }
	    if(buffer[0] == '-' && buffer[1] == '-' && buffer[2] == '-') {
	        s = &scanners[scannerCount];
	        s->beaconCount = 0;
	        s->links = malloc(64 * sizeof(struct scannerLink));
	        s->linkCount = 0;
	        sscanf(&buffer[12], "%d", &s->id);
	        scannerCount++;
	        continue;
	    }else{
	        struct beacon *b = &s->beacons[s->beaconCount];
	        
	        sscanf(strtok(buffer, ","), "%hd", &b->x);
	        sscanf(strtok(NULL, ","), "%hd", &b->y);
	        sscanf(strtok(NULL, ","), "%hd", &b->z);
	        b->ref = s;
	        
	        (s->beaconCount)++;
	        allBeaconsCount++;
	    }
	}
	fclose(file);
	
	printf("Found %d lines in %s.\n", lineCount, filename);
	printf("Found %d scanners in %s.\n", scannerCount, filename);
	printf("Found %d beacons in %s.\n", allBeaconsCount, filename);
	/*
	struct region *result = overlap(scanners[15], scanners[25]);
	printf("15 and 25 result %d\n", result->matchCount);
	printf("Region %d,%d,%d to %d,%d,%d\n", 
	    result->xMin, result->yMin, result->zMin,
	    result->xMax, result->yMax, result->zMax);
   
	result = overlap(scanners[25], scanners[15]);
	printf("25 and 15 result %d\n", result->matchCount);
	printf("Region %d,%d,%d to %d,%d,%d\n", 
	    result->xMin, result->yMin, result->zMin,
	    result->xMax, result->yMax, result->zMax);
	    
	for(int i=0; i<24; i++) {
	    struct beacon test = { 1, 2, 3 };
	    struct beacon r = rotateByStruct(test, uniqueRotations[i]);
	    printf("R %d  %d,%d,%d\n",i, r.x, r.y, r.z);
	}
	return 0;
	*/
	int uniqueBeaconsCount = allBeaconsCount;
	int overlaps[26] = { 0 };
	int overlapCount = 0;
	for(int i=0; i<scannerCount; i++) {
	    for(int j=i+1; j<scannerCount; j++) {
	        if(i==j) continue;
	        struct region *r = overlap(&scanners[i], &scanners[j]);
	        if(r->matchCount > 0) {
	            overlaps[i] = 1;
	            overlaps[j] = 1;
	            overlapCount++;
	            printf("Scanners %d and %d have %d overlapping beacons\n", i, j, r->matchCount);
	        }
	        uniqueBeaconsCount -= r->matchCount;
	        free(r);
	    }
	}
	
	printf("Unique beacons %d with %d overlaps\n", uniqueBeaconsCount, overlapCount);
	for(int i = 0; i<26; i++) {
	    printf("%d", overlaps[i]);
	}
	printf("\n");
	
	uniqueBeaconsCount = 0;
	struct beacon *uniqueBeacons = malloc(allBeaconsCount * sizeof(struct beacon));
	
	/*
	for(int i=0; i<scanners[0].beaconCount; i++) {
	    uniqueBeacons[uniqueBeaconsCount] = scanners[0].beacons[i];
	    uniqueBeaconsCount++;
	}
	*/
	struct beacon scannerZeroes[32] = { 0 };
	for(int s=0; s<scannerCount; s++) {
	    for(int l=0; l<scanners[s].linkCount; l++) {
	        struct scannerLink link = scanners[s].links[l];
	        //printf("Scanner %d has pos %d,%d,%d in frame of scanner %d\n",
	        //    link.scanner->id, link.position.x, link.position.y, link.position.z, s);
	        struct beacon scannerPos = { 0, 0, 0, link.scanner };
	        struct beacon zeroPos = transformToZero(scannerPos);
	        scannerZeroes[link.scanner->id] = zeroPos;
	        printf("Zcanner %d has pos %d,%d,%d in frame of scanner %d\n",
	            link.scanner->id, zeroPos.x, zeroPos.y, zeroPos.z, 0);
            printf("Rotation %d\n", link.r);
            printf("Link out from %d to %d\n", s, link.scanner->id);
	    }  
        //scannerToZero(&scanners[s]);
        printf("\n\n");
	    printf("Scanner %d\n", s);
		for(int i=0; i<scanners[s].beaconCount; i++) {
		    struct beacon *b = &scanners[s].beacons[i];
		    struct beacon bt = transformToZero(*b);
		    printf("B%d %d,%d,%d\n", s, bt.x, bt.y, bt.z);
		    
		    if(bt.ref == NULL) {
		        continue;
		    }
		    
		    bool unique = true;
		    for(int t=0; t<uniqueBeaconsCount; t++) {
		        struct beacon *ub = &uniqueBeacons[t];
                if(bt.x == ub->x && bt.y == ub->y && bt.z == ub->z) {
                    unique = false;
                    printf("U%d %d,%d,%d\n", s, bt.x, bt.y, bt.z);
                    break;
                }
	        }
	        
	        if(unique) {
                uniqueBeacons[uniqueBeaconsCount] = bt;
                uniqueBeaconsCount++;
            }
	    }
	}
	
	printf("Unique beacons %d using transform to zero\n", uniqueBeaconsCount);
	
	int maxDist = 0;
	for(int s1=0; s1<scannerCount; s1++) {
	    for(int s2=0; s2<scannerCount; s2++) {
	        if(s1 == s2) continue;
	        
	        struct beacon p1 = scannerZeroes[s1];
	        struct beacon p2 = scannerZeroes[s2];
	        
	        int d = (int)abs(p1.x - p2.x) + (int)abs(p1.y - p2.y) + (int)abs(p1.z - p2.z);
	        
	        if(d > maxDist) {
	            maxDist = d;
	        }
	    }
	}
	
	printf("Scanner max dist is %d\n", maxDist);
	
	for(int s=0; s<scannerCount; s++) {
        free(scanners[s].links);
	}
	free(scanners);

	return 0;
}
