import matplotlib.pyplot as plt
import sys
from pathlib import Path
import numpy as np
import struct

def draw_image(fileName):
    with open(fileName, 'rb') as file:
        width, height = struct.unpack('ii', file.read(4*2))
        image_data_bytes = file.read((width*height*4) * 4)
        image_data_float = struct.unpack('f'*(width*height*4), image_data_bytes)
        npimage = np.array(image_data_float).reshape((height, width, 4))[:,:,0:3]
        plt.imshow(npimage)
        plt.show()        

if __name__ == "__main__":
    ruta = Path(sys.argv[0]).parent.parent.parent
    fileName = Path(ruta,"Renderer","test.rbm")
    draw_image(fileName)