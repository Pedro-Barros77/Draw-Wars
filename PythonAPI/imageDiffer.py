from skimage.metrics import structural_similarity
import base64
import cv2
import os

def main(img_1, img_2):

    # Works well with images of different dimensions
    def orb_sim(img1, img2):
        # SIFT is no longer available in cv2 so using ORB
        orb = cv2.ORB_create()

        # detect keypoints and descriptors
        kp_a, desc_a = orb.detectAndCompute(img1, None)
        kp_b, desc_b = orb.detectAndCompute(img2, None)

        # define the bruteforce matcher object
        bf = cv2.BFMatcher(cv2.NORM_HAMMING, crossCheck=True)

        # perform matches.
        matches = bf.match(desc_a, desc_b)
        # Look for similar regions with distance < 50. Goes from 0 to 100 so pick a number between.
        similar_regions = [i for i in matches if i.distance < 50]
        if len(matches) == 0:
            return 0
        return len(similar_regions) / len(matches)

    orb_similarity = orb_sim(img_1, img_2)  # 1.0 means identical. Lower = not similar

    print("Similarity using ORB is: ", orb_similarity)
    
    return orb_similarity