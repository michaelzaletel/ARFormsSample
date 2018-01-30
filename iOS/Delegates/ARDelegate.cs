using UIKit;
using SceneKit;
using ARKit;
using Foundation;
using System;
using CoreGraphics;
using System.Linq;
using OpenTK;

namespace ARSample.iOS.Delegates
{
    public class ARDelegate : ARSCNViewDelegate
    {
        public override void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            if (anchor != null && anchor is ARPlaneAnchor)
            {
                PlaceAnchorNode(node, anchor as ARPlaneAnchor);
            }
        }

        //To place anchor node on the view
        void PlaceAnchorNode(SCNNode node, ARPlaneAnchor anchor)
        {
            var plane = SCNPlane.Create(anchor.Extent.X, anchor.Extent.Z);
            plane.FirstMaterial.Diffuse.Contents = UIColor.LightGray;
            var planeNode = SCNNode.FromGeometry(plane);

            //Locate the plane at the position of the anchor
            planeNode.Position = new SCNVector3(anchor.Extent.X, 0.0f, anchor.Extent.Z);
            //Rotate it to lie flat
            planeNode.Transform = SCNMatrix4.CreateRotationX((float)(Math.PI / 2.0));
            node.AddChildNode(planeNode);

            //Mark the anchor with a small red box
            var box = new SCNBox { Height = 0.1f, Width = 0.1f, Length = 0.1f };
            box.FirstMaterial.Diffuse.ContentColor = UIColor.Red;

            var anchorNode = new SCNNode { Position = new SCNVector3(0, 0, 0), Geometry = box };
            planeNode.AddChildNode(anchorNode);
        }

        //On node update
        public override void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            if (anchor is ARPlaneAnchor)
            {
                var planeAnchor = anchor as ARPlaneAnchor;
                //BUG: Extent.Z should be at least a few dozen centimeters
                System.Console.WriteLine($"The (updated) extent of the anchor is [{planeAnchor.Extent.X}, {planeAnchor.Extent.Y}, {planeAnchor.Extent.Z}]");
            }
        }
    }
}
