

bibleSections = open(r'/Users/shanedeiley/Dropbox/Data_Science_Practice/Hackathon/c4tkmicrosoft/resources/bibleSections.txt')
bibleJson = open(r'/Users/shanedeiley/Dropbox/Data_Science_Practice/Hackathon/c4tkmicrosoft/resources/bibleSections.json', 'w')


book_name = ""
old_name = ""
chapter_number = 0
title = ""

# Start the Json
bibleJson.write("{")

for line in bibleSections:

    # Get rid of trailing newline
    line = line[:-1]
    
    array = line.split(' ')
    array2 = line.split(':')

    # Info
    book_name = array[0]
    chapter_number = array[1][:-1]
    title = array2[1][1:]

    # Create Json for info
    if old_name == book_name: # Continue adding list objects to chapter
        bibleJson.write(",{\"" + str(chapter_number) + "\":\"" + str(title) + "\"}")

    else:
        # New Book Reached
        if str(book_name) == "Genesis": # Start directly with book name entry
            bibleJson.write("\"" + str(book_name) + "\":[")
        else: # End last list, start new book name entry
            bibleJson.write("],\"" + str(book_name) + "\":[") 

        # Add first list entry
        bibleJson.write("{\"" + str(chapter_number) + "\":\"" + str(title) + "\"}")
        old_name = book_name

# Finally, close last line
bibleJson.write("]}")

bibleJson.close()
bibleSections.close()
