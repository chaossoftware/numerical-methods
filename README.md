# CHAOSSOFT #
![chaossoft](./icon.png)

Chaossoft is a set of libraries and tools which allow to model systems of different nature and to perform numeric analysis of timeseries.  
#
![image](https://user-images.githubusercontent.com/3007588/216699510-040c0477-6688-4856-bf9f-ae64a98228db.png)  
Novel method to compute LEs based on a modification of the neural network method was designed, implemented in **NeuralNetTsa** tool and **[published](https://www.mdpi.com/1099-4300/20/3/175)** in Entropy journal

#
### ChaosSoft.Core ###
Core of the toolset.

* Custom data series implementation
* Data I/O
* Numerical methods:  
-- Lyapunov exponents related methods  
-- equations solving  
-- phase space reconstruction  
-- orthogonalization  
* Methods from algebra
* Methods from statistics

### TsaToolbox ###
**T**ime **S**eries **A**nalysis **Toolbox**
* Timeseries load and customization of range for analysis
* variety of plots to build (interactive zoomable + enlarged popups):  
-- signal  
-- pseudo-poincare section  
-- autocorrelation  
-- mutual information  
-- false nearest neighbors  
-- wavelets*  
-- FFT*  
\* _matlab compiler runtime 2016b needs to be installed_  

* Lyapunov exponents analysis:  
-- wolf  
-- rosenstein  
-- kantz  
-- sano-sawada

* and some other cool features  
![image](https://user-images.githubusercontent.com/3007588/215598157-78cfb33e-c3f1-4d32-b14e-35abfc993e92.png)
![image](https://user-images.githubusercontent.com/3007588/215598217-7b96639b-c24d-4948-a929-e9237992463f.png)


### NeuralNetTsa ###

**Neural Net** **T**ime **S**eries **A**nalysis 
CLI tool which performs forecasting of timeseries, reconstruction of the attractor and calculating some attractor properties  
![lorenz_neural_anim - Copy (3)](https://user-images.githubusercontent.com/3007588/216692942-e23af69b-19a5-41ad-9ecb-f9bf1b02269e.gif)


### Attractor viewer ###
The tool to visualize attractors in 3D.  
![image](https://user-images.githubusercontent.com/3007588/216684098-041eb1da-e405-4976-992f-b4bd87e37bf0.png)


### Modelled Systems ###
Big variety of "classic" well-known equatinos systems with ability to perform analysis of different kind
* Map of lyapunov exponents
* LLE and Lyapunov spectrum
* Lyapunov fractals
* Bifucration maps
