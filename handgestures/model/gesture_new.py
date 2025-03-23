import cv2
import numpy as np
import mediapipe as mp
import tensorflow as tf
import socket

# Load trained model
model = tf.keras.models.load_model('model/keypoint_classifier/keypoint_classifier2.keras')

# Gesture labels (Modify if needed)
gesture_labels = ['open', 'close', 'side', 'index', 'thumbsdown']

# UDP Setup
UDP_IP = "127.0.0.1"  # Localhost
UDP_PORT = 5008
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# MediaPipe Hand Tracking
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(
    max_num_hands=1,
    min_detection_confidence=0.7,
    min_tracking_confidence=0.5
)
mp_drawing = mp.solutions.drawing_utils

# Function to preprocess landmarks for model prediction
def preprocess_landmarks(landmarks):
    base_x, base_y = landmarks[0][0], landmarks[0][1]
    processed = []
    for x, y in landmarks:
        processed.append(x - base_x)
        processed.append(y - base_y)
    max_value = max(max(processed), abs(min(processed)))
    processed = np.array(processed) / max_value  # Normalizing
    return np.expand_dims(processed, axis=0)  # Model expects batch dimension

# Function to predict gesture
def predict_gesture(landmarks):
    processed_landmarks = preprocess_landmarks(landmarks)
    prediction = model.predict(processed_landmarks)
    gesture_id = np.argmax(prediction)
    confidence = prediction[0][gesture_id]
    if confidence > 0.8:  # Adjust threshold based on your model
        return gesture_labels[gesture_id]
    else:
        return "Unknown"

def main():
    cap = cv2.VideoCapture(0)

    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break

        # Flip frame for better user experience
        frame = cv2.flip(frame, 1)
        frame = cv2.resize(frame, (240, 180))
        image_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        result = hands.process(image_rgb)

        if result.multi_hand_landmarks:
            for hand_landmarks in result.multi_hand_landmarks:
                # Extract landmark points (x, y only)
                landmarks = []
                for lm in hand_landmarks.landmark:
                    landmarks.append([lm.x, lm.y])

                # Predict gesture
                gesture = predict_gesture(landmarks)

                # Send detected gesture to Unity via UDP
                if gesture != "Unknown":
                    sock.sendto(gesture.encode(), (UDP_IP, UDP_PORT))
                    print(f"Sent Gesture: {gesture}")

                # Draw landmarks and detected gesture on screen
                mp_drawing.draw_landmarks(frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)
                cv2.putText(frame, f'Gesture: {gesture}', (10, 50),
                            cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

        cv2.imshow("Hand Gesture Recognition", frame)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    cap.release()
    cv2.destroyAllWindows()

if __name__ == '__main__':
    main()
