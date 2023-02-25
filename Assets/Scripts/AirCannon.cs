using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCannon : MonoBehaviour
{
    public float airCannonStrength = 1.0f;

    /*Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus ut fringilla libero, suscipit ultrices nisl. Pellentesque euismod lacus quis erat lacinia vehicula. Nam fringilla nibh nunc, vitae auctor lectus porta vitae. Ut lobortis lobortis nibh, in pharetra risus blandit et. Pellentesque dignissim laoreet risus at luctus. Quisque at sodales orci. Suspendisse at tellus ac tortor finibus faucibus. Vivamus eget condimentum velit. Nam at quam vitae elit bibendum gravida. Ut eleifend vel libero et congue. Fusce ut accumsan nulla. Aliquam eget nisi laoreet purus pretium pretium sit amet ut eros. Donec diam velit, consequat et mattis nec, malesuada et purus. Sed non nunc aliquam, dapibus ipsum sed, cursus erat. Proin feugiat ipsum magna, a venenatis arcu eleifend a. Quisque vel metus tortor.

Donec non augue metus. Aliquam interdum blandit blandit. Sed eget nibh quis quam pulvinar finibus condimentum at ante. Fusce luctus justo ac commodo malesuada. In in est et sapien euismod lobortis. Vestibulum fringilla magna nec nibh vulputate, nec faucibus justo pellentesque. Fusce sit amet diam mollis, pretium sem eu, tincidunt nisl. Suspendisse condimentum tincidunt luctus. Vestibulum in sodales purus, a rutrum nunc. Ut elit mi, mollis ut interdum eu, aliquet quis metus. Duis vitae neque neque. Sed in ipsum ut enim efficitur varius nec quis odio. Suspendisse quis ultrices lacus. Mauris a erat non purus tempus imperdiet sit amet eget neque.

Etiam ex nunc, ornare vitae lectus vel, accumsan tempus enim. Curabitur dapibus ultrices bibendum. In hac habitasse platea dictumst. Maecenas blandit justo a augue fringilla luctus. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Proin eget dictum neque, fermentum tristique turpis. Vestibulum pulvinar nisl a dolor vestibulum, a porttitor eros elementum.

Ut varius elit nec varius fermentum. Phasellus pharetra maximus velit, eu auctor magna. Integer sit amet venenatis odio. Vestibulum maximus quis velit id convallis. Sed sed ornare risus, in bibendum nibh. Integer elit ipsum, gravida at metus sed, placerat tempus sapien. Morbi rhoncus, tellus ac volutpat rutrum, arcu erat dapibus risus, in pellentesque purus nunc aliquam dolor. Donec tincidunt scelerisque metus vitae hendrerit. Integer tempus lacus neque, nec mollis felis euismod nec.

In rutrum ultricies orci. Proin facilisis erat et metus ullamcorper rhoncus. Pellentesque iaculis ante vitae tortor placerat, quis consequat quam semper. Sed lobortis dui eget tortor hendrerit, non tempus erat cursus. Mauris eu dignissim ante, in finibus massa. Phasellus molestie ultricies justo, efficitur laoreet ante tempor mattis. Morbi varius aliquam nisl, non suscipit ipsum consequat ut. Vivamus facilisis metus in pretium imperdiet. Sed eu tempus lectus. Vivamus mattis magna nisi, ac accumsan mi rhoncus eleifend. Maecenas egestas lectus id justo suscipit tempus. Phasellus finibus et elit sed accumsan. Proin nec tortor maximus, mattis nulla gravida, rutrum purus. Donec velit lacus, laoreet at mauris ac, sodales tristique elit.*/


    public Vector3 Boost()
    {
        return transform.forward.normalized * airCannonStrength;
    }
}
