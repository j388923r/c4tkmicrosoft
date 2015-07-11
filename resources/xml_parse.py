# Eric Gan
# Parses the XML file ESV.xml. Creates a new file esv.txt containing
# the plaintext.

import xml.etree.ElementTree as ET

if __name__ == '__main__':
    tree = ET.parse("ESV.xml")
    bible = tree.getroot()
   
    corpus = "" 
    
    for book in bible:
        for chapter in book:
            for verse in chapter: 
                corpus += verse.text + " "
                
    outputfd = open("esv.txt", "w")
    outputfd.write(corpus)
    outputfd.close()