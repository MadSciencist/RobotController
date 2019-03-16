clear;
close all;

log = csvread('sample_log.csv', 1, 1); % skip first row (headers) and first col (timestamp, not properly formated for matlab)
l_setpoint = log(:,1);
l_vel = log(:,5);

vel_considered_range = l_vel(19:79);

SAMPLING_RATE = 75; % 75 ms
len = length(vel_considered_range);
time = ((0:len-1)*SAMPLING_RATE)/1000;
time = time';

fsamp = 13.3333; % sampling frequency
f_cutoff = 4.4;  % examples on 4.4, 0.7, 2.5
fnorm = f_cutoff / fsamp; % normalize
[b,a] = cheby1(1, 2, 2*fnorm);
vel_filtered = filter(b, a, vel_considered_range);

plot(time, vel_considered_range)
hold on;
plot(time, vel_filtered);
xlabel('Time [sec]');
ylabel('Angular velocity [P.U.]');
legend('RAW velocity','Filtered velocity')
set(gcf,'color','w');