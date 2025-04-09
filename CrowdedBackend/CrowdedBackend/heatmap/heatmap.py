# For installing envoirnment write pip3 install -r requirements.txt

import numpy as np
import matplotlib.pyplot as plt
import matplotlib.image as mpimg
import time
import os
import requests
from scipy.stats import gaussian_kde


def generateHeatmap(raspberryPiPosition: list[list[int]], peoplePosition: list[list[int]]):
    # Load the floor plan
    map_path = "/Users/davidfraenkel/heatmap/heatmap.drawio.png"
    map_img = mpimg.imread(map_path)

    # Sample positions of detected people
    # raspberryPiPosition = [[8,2],[6,5],[10,10]]

    # Sample positions of detected people
    # peoplePosition = [[2,3],[2,5],[1,7],[4,8],[1,5],[1,6],[1,1.5],[7,8],[9,8],[10,10]]

    # Convert to a properly formatted NumPy array
    positions_array = np.array(peoplePosition)

    # Extract X and Y coordinates
    x, y = positions_array[:, 0], positions_array[:, 1]

    # Generate KDE (heatmap)
    kde_heatmap = gaussian_kde(np.vstack([x, y]))
    xmin, xmax, ymin, ymax = 0, 12, 0, 12
    xx, yy = np.mgrid[xmin:xmax:100j, ymin:ymax:100j]
    density = kde_heatmap(np.vstack([xx.ravel(), yy.ravel()])).reshape(xx.shape)
    # plot the raspberryPi positions

    # Plot the heatmap overlay
    fig, ax = plt.subplots(figsize=(12, 12))

    # Display floor plan
    ax.imshow(map_img, extent=[xmin, xmax, ymin, ymax], alpha=0.6)

    # Overlay heatmap (store the result in 'heatmap' to use with colorbar)
    heatmap = ax.imshow(density.T, origin='lower', extent=[xmin, xmax, ymin, ymax], cmap='jet', alpha=0.5)

    # Scatter plot for raspberry pi positions
    for position in raspberryPiPosition:
        ax.scatter(position[0], position[1], c='white', edgecolors='black', label='Raspberry Pi Position')

    # Add colorbar using the heatmap
    plt.colorbar(heatmap, ax=ax, label="Density")

    # Get epoch time in seconds and convert to string
    seconds = time.time().__str__()
    stripped_seconds = seconds.split(".")[0]

    # Save the image
    plt.savefig(stripped_seconds + ".png", dpi=300)

    # Api to server
    url = "Test url" # <- replace this with backend api

    # Image path
    image_path = stripped_seconds + ".png"

    # Send to server
    with open(image_path, 'rb') as img_file:
        files = {'image': (image_path, img_file, 'image/png')}
        response = requests.post(url, files=files)

    # Check response
    if response.status_code == 200:
        # delete image
        os.remove(image_path)
        print("Image sent successfully and deleted.")
