import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
import sys




def main():
    file = sys.argv[1:][0]
    print(file)
    plot(file)


def load_prepare_data(filename):
    data = pd.read_csv(filename)
    left_set = data.iloc[:, 1].values.astype(float)
    right_set = data.iloc[:, 2].values.astype(float)
    left_speed = data.iloc[:, 3].values.astype(float)
    right_speed = data.iloc[:, 4].values.astype(float)
    left_speed_raw = data.iloc[:, 5].values.astype(float)
    right_speed_raw = data.iloc[:, 6].values.astype(float)
    left_curr = data.iloc[:, 7].values.astype(float)
    right_curr = data.iloc[:, 8].values.astype(float)
    left_curr_raw = data.iloc[:, 9].values.astype(float)
    right_curr_raw = data.iloc[:, 10].values.astype(float)
    return left_set, right_set, left_speed, right_speed, left_speed_raw, right_speed_raw, left_curr, right_curr, left_curr_raw, right_curr_raw

def plot(path):
    SAMPLING_PERIOD = 0.075 # 75 ms
    l_s, r_s, l_speed, r_speed, l_raw_speed, r_raw_speed, l_curr, r_curr, l_raw_curr, r_raw_curr = load_prepare_data(path)
    time = np.arange(0, len(l_s)*SAMPLING_PERIOD, SAMPLING_PERIOD)
    plt.plot(time,l_s, label='Setpoint')
    plt.plot(time, l_raw_speed, label='Right velocity')
    plt.plot(time, r_raw_speed, label='Right velocity')
    plt.xlabel('time [s]')
    plt.ylabel('Velocity [p.u.]')
    plt.title('Velocity')
    plt.legend()
    plt.show()
	
	
if __name__ == "__main__":
    main()