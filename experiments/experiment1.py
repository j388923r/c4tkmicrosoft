import numpy
from scipy.spatial import distance
import math
import statistics

def read():
    f = open('../resources/h_nmf.csv', 'r')
    n=1189
    h_T = [[] for i in range(n)]
    for line in f:
        values=line[:len(line)-2].replace("\n", "").split(",")
        #print(values)
        for i, value in enumerate(values):
            h_T[i].append(float(value))
    #print("here")
    return h_T

def getTopIndices(h_T, chapterIndex, k):
    
    vector = h_T[chapterIndex]
    vectorI = []
    for i, value in enumerate(vector):
        vectorI.append([i, value])
    #for i, value in enumerate
    vectorSorted=sorted(vectorI, key=lambda x:x[1], reverse=True)
    indices =[]
    for i in range(0, min(k, len(vector))):
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
    Matrix = [[0.0 for x in range(len(h_T))] for x in range(len(h_T))] 
    for i in range(0, len(h_T)):
        for j in range(0, len(h_T)):
            if i!=j:
                indices = getTopIndices(h_T, i, k)
                #print(indices)
                #print("i="+str(i))
                #print("j="+str(j))
                vector_i=[h_T[i][a] for a in indices]
                vector_j=[h_T[j][a] for a in indices]
                dist = distance.euclidean(vector_i,vector_j)
                Matrix[i][j]=dist
                if(smallDistances[i][0]>dist):
                    smallDistances[i][0]=dist
                    smallDistances[i][1]=j

    bigDistances = []
    differences=[]
    for i in range(0, len(h_T)):
        sumDist = 0.0
        for j in range(0, len(h_T)):
            if i!=j and smallDistances[i][1]!=j:
                bigDistances.append(Matrix[i][j])
                sumDist+=Matrix[i][j]
        sumDist=sumDist/1188
        dif =sumDist - smallDistances[i][0]
        differences.append(dif)
        print("differences: "+str(dif))
    difference = numpy.mean(numpy.array(bigDistances))-numpy.mean(numpy.array(smallDistances))       
    return sum(differences)




def timeComplexity(h_T, k):
    distances = 1
    for i in range(0, len(h_T)):
        for j in range(0, len(h_T)):
            if i!=j:
                distances=i
    return distances
def main():
    h_T = read()
    #indices = getTopIndices(h_T, 1, 10)
    #Ndistances = getNDistances(h_T, 10)
    differences = []
    fout = open('results.txt', 'w')
    for i in range(1, 10):
        print("i="+str(i))
        diff = getNDistances(h_T, i)
        differences.append(diff)
        fout.write(str(diff)+",")
        print("diff: "+str(diff))
    diff = getNDistances(h_T, 100000)
    differences.append(diff)
    fout.write(str(diff)+",")
    print("diff: "+str(diff))
if __name__ == "__main__":
    main()
