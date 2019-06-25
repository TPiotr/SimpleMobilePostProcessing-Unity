# Simple Mobile Post Processing for Unity
Idea of this project is that I was bored of standard post processing stack for unity which is unoptimized for mobile devices (applying simple vingette effect adds 6ms in frame time, wholescreen blur is destroying framerate totally) To solve this I created this simple post processing which currently only offers two post processing effect:
- **Whole Screen Blur** - you can control blur radius and texture resoulution scale
- **Vignette** - you can control effect intensity and center of vignette

Compared to default unity post processing stack this approach don't really have performance hit.

# Presentation
![alt text](https://github.com/TPiotr/SimpleMobilePostProcessing-Unity/blob/master/presentation/showcase1_gif.gif)

# How to use
Download CustomPostProcessing folder put it in your project and drag PostProcessor into your Camera thats all. You can change all effects parameters from your script during runtime by grabbing PostProcessor reference and modifying variables like this:
```
public PostProcessor postProcessor;
void Update() {
    postProcessor.BlurRadius = Mathf.sin(Time.time);
}
```
