using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SceneKit;
using UIKit;
using ARKit;
using CoreGraphics;
using Foundation;
using OpenTK;
using System.Linq;
using ARSample.Controls.AR;
using ARSample.iOS.CustomRenderers.ARPage;
using System;
using ARSample.iOS.Delegates;

[assembly: ExportRenderer(typeof(ARPage), typeof(ARPageRenderer))]

namespace ARSample.iOS.CustomRenderers.ARPage
{

    //Page Renderer to display AR Screen View from Forms Code, implementing AR ScreenView Delegate
    public class ARPageRenderer : PageRenderer, IARSCNViewDelegate
    {
        ARSCNView scnView;

        public ARPageRenderer() : base()
        {
            //nothing to do in constructor
        }

        public override bool ShouldAutorotate() => true;

        //On screen view loaded
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //initializing AR Screen View
            scnView = new ARSCNView()
            {
                Frame = this.View.Frame,
                Delegate = new ARDelegate(),
                DebugOptions = ARSCNDebugOptions.ShowFeaturePoints | ARSCNDebugOptions.ShowWorldOrigin,
                UserInteractionEnabled = true
            };

            //Adding subview to this page view
            this.View.AddSubview(scnView);
        }

        //Before showing AR Screen View to user
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Configure ARKit 
            var config = new ARWorldTrackingConfiguration();
            config.PlaneDetection = ARPlaneDetection.Horizontal;

            // This method is called subsequent to `ViewDidLoad` so we know `scnView` is instantiated
            scnView.Session.Run(config, ARSessionRunOptions.RemoveExistingAnchors);
        }

        //On view interaction
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            var touch = touches.AnyObject as UITouch;
            if (touch != null)
            {
                //getting location of touch
                var loc = touch.LocationInView(scnView);
                //position in world 
                var worldPos = WorldPositionFromHitTest(loc);

                //if corresponds to some world position
                if (worldPos.Item1.HasValue)
                {
                    //Add cube on AR Screen View
                    PlaceCube(worldPos.Item1.Value);
                }
            }
        }

        private SCNVector3 PositionFromTransform(NMatrix4 xform)
        {
            return new SCNVector3(xform.M14, xform.M24, xform.M34);
        }

        //Getting world position from touch hit
        Tuple<SCNVector3?, ARAnchor> WorldPositionFromHitTest(CGPoint pt)
        {
            //Hit test against existing anchors
            var hits = scnView.HitTest(pt, ARHitTestResultType.ExistingPlaneUsingExtent);
            if (hits != null && hits.Length > 0)
            {
                var anchors = hits.Where(r => r.Anchor is ARPlaneAnchor);
                if (anchors.Count() > 0)
                {
                    var first = anchors.First();
                    var pos = PositionFromTransform(first.WorldTransform);
                    return new Tuple<SCNVector3?, ARAnchor>(pos, (ARPlaneAnchor)first.Anchor);
                }
            }
            return new Tuple<SCNVector3?, ARAnchor>(null, null);
        }

        //To load assets  
        private SCNMaterial[] LoadMaterials()
        {
            Func<string, SCNMaterial> LoadMaterial = fname =>
            {
                var mat = new SCNMaterial();
                mat.Diffuse.Contents = UIImage.FromFile(fname);
                mat.LocksAmbientWithDiffuse = true;
                return mat;
            };

            var a = LoadMaterial("msft_logo.png");
            var b = LoadMaterial("xamagon.png");
            var c = LoadMaterial("fsharp.png"); // This demo was originally in F# :-) 

            return new[] { a, b, a, b, c, c };
        }

        SCNNode PlaceCube(SCNVector3 pos)
        {
            var box = new SCNBox { Width = 0.25f, Height = 0.25f, Length = 0.25f };
            var cubeNode = new SCNNode { Position = pos, Geometry = box };
            cubeNode.Geometry.Materials = LoadMaterials();
            scnView.Scene.RootNode.AddChildNode(cubeNode);
            return cubeNode;
        }
    }
}
