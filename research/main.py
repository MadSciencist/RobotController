import matplotlib.pyplot as plt
import pandas as pd
import numpy as np

SAMPLING_PERIOD = 0.075 # 75 ms

def load_prepare_data(filename):
    data = pd.read_csv(filename)
    left_set = data.iloc[:, 0].values.astype(float)
    right_set = data.iloc[:, 1].values.astype(float)
    left_speed = data.iloc[:, 2].values.astype(float)
    right_speed = data.iloc[:, 3].values.astype(float)
    left_speed_raw = data.iloc[:, 4].values.astype(float)
    right_speed_raw = data.iloc[:, 5].values.astype(float)
    left_curr = data.iloc[:, 6].values.astype(float)
    right_curr = data.iloc[:, 7].values.astype(float)
    left_curr_raw = data.iloc[:, 8].values.astype(float)
    right_curr_raw = data.iloc[:, 9].values.astype(float)
    return left_set, right_set, left_speed, right_speed, left_speed_raw, right_speed_raw, left_curr, right_curr, left_curr_raw, right_curr_raw

l_s, r_s, l_speed, r_speed, l_raw_speed, r_raw_speed, l_curr, r_curr, l_raw_curr, r_raw_curr = load_prepare_data('./pid.csv')

time = np.arange(0, len(l_s)*SAMPLING_PERIOD, SAMPLING_PERIOD)


plt.plot(time,l_s, label='Setpoint')
plt.plot(time, l_raw_speed, label='Right velocity')
plt.plot(time, r_raw_speed, label='Right velocity')
plt.xlabel('time [s]')
plt.ylabel('Velocity [p.u.]')
plt.title('Velocity')
plt.legend()
plt.show()