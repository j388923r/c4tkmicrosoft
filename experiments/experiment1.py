import numpy
from scipy.spatial import distance

def read():
    f = open('../resources/h_nmf.csv', 'r')
    n=1190
    h_T = [[]]*n
    for line in f:
        values=line[:len(line)-2].replace("\n", "").split(",")
        print(values)
        for i, value in enumerate(values):
            h_T[i].append(float(value))
    print("here")
    return h_T

def getTopIndices(h_T, chapterIndex, k):
    
    vector = h_T[chapterIndex]
    vectorI = []
    for i, value in enumerate(vector):
        vectorI.append([i, value])
    #for i, value in enumerate
    vectorSorted=sorted(vectorI, key=lambda x:x[1], reverse=True)
    indices =[]
    for i in range(0, k):
        indices.append(vectorSorted[i][0])
    #print(indices)
    return indices

def getAllDistances(h_T, k, NDist):
    distances = []
    for i in range(0, len(h_T)):
        for j in range(0, len(h_T)):
            if i!=j and NDist[i][1]!=j:
                indices = getTopIndices(h_T, i, k)
                vector_i=[h_T[i][a] for a in indices]
                vector_j=[h_T[j][a] for a in indices]
                dist = distance.euclidean(vector_i,vector_j)
                distances.append(dist)

def getNDistances(h_T, k):
    smallDistances= [[10000000, -1]]*len(h_T)
    Matrix = [[0 for x in range(len(h_T))] for x in range(len(h_T))] 
    for i in range(0, len(h_T)):
        for j in range(0, len(h_T)):
            if i!=j:
                indices = getTopIndices(h_T, i, k)
                vector_i=[h_T[i][a] for a in indices]
                vector_j=[h_T[j][a] for a in indices]
                dist = distance.euclidean(vector_i,vector_j)
                Matrix[i][j]=dist
                if(smallDistances[i][0]>dist):
                    smallDistances[i][0]=dist
                    smallDistances[i][1]=j

    bigDistances = []
    for i in range(0, len(h_T)):
        for j in range(0, len(h_T)):
            if i!=j and smallDistances[i][1]!=j:
                bigDistances.append(Matrix[i][j])
    difference = math.abs(numpy.mean(bigDistances)-numpy.mean(smallDistances))       
    return difference




def timeComplexity(h_T, k):
    distances = 1
    for i in range(0, len(h_T)):
        for j in range(0, len(h_T)):
            if i!=j:
                distances=i
    return distances
def main():
    h_T = read()
    indices = getTopIndices(h_T, 1, 10)
    Ndistances = getNDistances(h_T, 10)
    getAllDistances(h_T, k, NDist)

if __name__ == "__main__":
    main()
