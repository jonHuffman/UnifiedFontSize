# UnifiedFontSize
This is a small Component for Unity designed to allow you to link multiple Text objects and have them share a best fitted font size. On Start() the component will scan the linked Text components and determine the smallest font size of the set. It then applies this value to the entire collection, resulting in a more uniform look.

![Unified Size Demo](ReadMeImages/UnifiedSizes.gif?raw=true)

## Linking Text Objects
The UnifyTextSize component needs a reference to all the Text components that you wish it to affect. The easiest way to accomplish this is to simply drag and drop the Text objects into the List found in the UnifyTextSize component.  
For organization purposes, it is recommended that the UnifyTextSize component be attanched to a GameObject other than those it affects.
![How to Link](ReadMeImages/LinkingTextObjects.gif?raw=true)

## Adding Text Object at Runtime
It is also possible to add Text components to the List tracked by the UnifyTextSize component at runtime. With a reference to the component simply call the AddText(Text) method passing in the new Text object that you wish tracked.  
When a new Text object is added to UnifyTextSize's List, it will automatically check to see if it has a smaller font size than the existing group, and will update itself and the group accordingly.

## Complete Recalculation of Best Fit
In the event that you are changing the content of your Text components at runtime you will likely wish to re-calculate the group's font size. If you do not and your new values allow for a larger collective font size, they will not update and instead will use the previously calculated size.  
Recalculating the group's font size can be done by calling either the `RecalculateBestFit()` or `RecalculateBestFitImmediately()` method.  

        private void Recalculate()
        {
            UnifyTextSize unifyTextSizeController = GetComponent<UnifyTextSize>();
            unifyTextSizeController.RecalculateBestFit();
        }

## RecalculateBestFit vs RecalculateBestFitImmediately
In order to determine the best fit value of the group of Text component's, Unity needs to update the Canvas. Canvas recalcuation can be an expensive operation and as such we typically want to avoid it as often as possible. To this end, UnifyTextSize gives you the options of either `RecalculateBestFit()` or `RecalculateBestFitImmediately()`.
* **RecalculateBestFit** will run across two frames, allowing for Unity to naturally update the Canvas during its frame cycle. The benefit is that you do not incur an additional Canvas update. The disadvantage is that for a frame your text sizes will not be uniform.  
* **RecalculateBestFitImmediately** calculates the new font size immediately. The benefit is that the new size is applied in a single frame, meaning the user will never see a non-uniform size. The disadvantage is that you incur a second Canvas update in the frame that this is called.  

> Currently UnityEngine's `LayoutRebuilder` does not cause font size to be recalculated. Its possible this will change in future Unity versions. (Last tested in Unity 2018.2.15)

### Choosing a Recalculation option for Initialization
UnifiedFontSize runs a size recalculation in the Start() method of the component in order to unify the font size of all Text objects immedaitely. By default it does so through the `RecalculateBestFitImmediately()` method. In the inspector you can instead uncheck the *Unify Immediately* toggle on the UnifyTextSize component and it will instead use `RecalculateBestFit()`.