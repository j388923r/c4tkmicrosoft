# Eric Gan
# Parses the XML file ESV.xml. Creates a new file esv.txt containing
# the plaintext.

import xml.etree.ElementTree as ET
import sys

if __name__ == '__main__':
    if len(sys.argv) != 3: 
        print "Usage: %s <inFile> <outFile>" % (sys.argv[0])
        exit(0)

    inFile = sys.argv[1]
    outFile = sys.argv[2]

    tree = ET.parse(inFile)
    bible = tree.getroot()
   
    corpus = "" 
    
    for book in bible:
        for chapter in book:
            for verse in chapter: 
                corpus += verse.text + " "
                
    with open(outFile, "w") as outputfd: 
        outputfd.write(corpus)
