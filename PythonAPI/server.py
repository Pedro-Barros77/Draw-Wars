import time
import cv2
import numpy as np
import base64
import zmq
import imageDiffer

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://192.168.18.211:5555")

byteArray1 = ""
byteArray2 = ""

while True:
    #  Wait for next request from client
    message = socket.recv()
    #print("Received request: " + message)

    def deserializeImg(data):
        npimg = np.fromstring(data, dtype=np.uint8); 
        result = cv2.imdecode(npimg, 1)
        return result

    if len(byteArray1) > 0:
        byteArray2 = message
    else:
        byteArray1 = message
        socket.send(b"")
        continue

    image1 = deserializeImg(byteArray1)
    image2 = deserializeImg(byteArray2)
    
    result = ""


    result = str(imageDiffer.main(image1, image2))
    
    socket.send_string(result)

    byteArray1 = ""
    byteArray2 = ""
