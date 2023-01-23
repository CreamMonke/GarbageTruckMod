using UnityEngine; 

namespace Mod
{
    public class Mod
    {
        public static void Main()
        {
            ModAPI.Register(
                new Modification()
                {
                    OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                    NameOverride = "Garbage Truck",
                    DescriptionOverride = "A truck with garbage.",
                    CategoryOverride = ModAPI.FindCategory("Vehicles"),
                    ThumbnailOverride = ModAPI.LoadSprite("preview.png"),
                    AfterSpawn = (Instance) =>
                    {
                        //TRUCK BODY
                        Instance.GetComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite("truckBodyCol.png");
                        Instance.gameObject.FixColliders();
                        Instance.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("truckBody.png");
                        Instance.GetComponent<SpriteRenderer>().sortingOrder=0;
                        Instance.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");//https://www.studiominus.nl/ppg-modding/details/layers.html
                        float direction = Instance.transform.localScale.x;
                        Instance.GetComponent<Rigidbody2D>().mass=5000f;


                        //BACK END
                        HingeJoint2D back = GameObject.Instantiate(ModAPI.FindSpawnable("Metal Cube").Prefab, Instance.transform.position + new Vector3(-4.7f*direction, 0.1f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        back.transform.localScale=new Vector3(direction, 1f, 1f);
                        back.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("backCol.png");
                        back.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                        back.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        back.GetComponent<Rigidbody2D>().mass = 500f;
                        back.gameObject.FixColliders();
                        back.GetComponent<SpriteRenderer>().sprite = ModAPI.LoadSprite("back.png");
                        back.connectedBody = Instance.GetComponent<Rigidbody2D>();
                        back.anchor = new Vector2(1.5f, 2.2f);
                        back.breakForce=25000f;

                        JointAngleLimits2D jal = new JointAngleLimits2D();
                        jal.min = direction<0?-60:-1;
                        jal.max = direction<0?0:60;
                        back.limits = jal;

                        back.gameObject.AddComponent<GarbageTruckBack>().direction=direction;

                        //This is for debugging
                        // GameObject debugger = new GameObject();
                        // debugger.AddComponent<SpriteRenderer>().sprite=ModAPI.LoadSprite("locator.png");
                        // debugger.GetComponent<SpriteRenderer>().sortingOrder=1;
                        // debugger.transform.parent=back.transform;
                        // debugger.transform.localPosition=new Vector2(1.5f, 2.2f);

                        //GRINDERS
                        GameObject w = ModAPI.FindSpawnable("Wheel").Prefab;

                        GameObject grinder1 = GameObject.Instantiate(w, back.transform.position + new Vector3(-0.7f*direction, -0.7f, 0f), Quaternion.identity);
                        grinder1.transform.localScale*=1.6f;
                        grinder1.GetComponent<Rigidbody2D>().mass=0f;
                        grinder1.GetComponent<PhysicalBehaviour>().Selectable=false;
                        WheelJoint2D grinderJoint = grinder1.AddComponent<WheelJoint2D>();
                        grinderJoint.connectedBody = back.GetComponent<Rigidbody2D>();
                        grinderJoint.connectedAnchor = new Vector3(-0.7f, -0.7f, 0f);
                        JointSuspension2D grinderJs = grinderJoint.suspension;
                        grinderJs.dampingRatio = 0f;
                        grinderJs.frequency = 1000000f;
                        grinderJoint.suspension = grinderJs;
                        JointMotor2D grinderMotor = new JointMotor2D();
                        grinderMotor.motorSpeed=250*direction;
                        grinderMotor.maxMotorTorque=250;
                        grinderJoint.motor = grinderMotor;
                        grinderJoint.useMotor=true;
                        GameObject.Destroy(grinder1.GetComponent<SpriteRenderer>());

                        GameObject grinder2 = GameObject.Instantiate(w, back.transform.position + new Vector3(-0.16f*direction, 0.8f, 0f), Quaternion.identity);
                        grinder2.transform.localScale*=1.4f;
                        grinder2.GetComponent<Rigidbody2D>().mass=0f;
                        grinder2.GetComponent<PhysicalBehaviour>().Selectable = false;
                        WheelJoint2D grinder2Joint = grinder2.AddComponent<WheelJoint2D>();
                        grinder2Joint.connectedBody = back.GetComponent<Rigidbody2D>();
                        grinder2Joint.connectedAnchor = new Vector3(-0.16f, 0.8f, 0f);
                        JointSuspension2D grinder2Js = grinder2Joint.suspension;
                        grinder2Js.dampingRatio = 0f;
                        grinder2Js.frequency = 1000000f;
                        grinder2Joint.suspension = grinder2Js;
                        JointMotor2D grinder2Motor = new JointMotor2D();
                        grinder2Motor.motorSpeed=-250*direction;
                        grinder2Motor.maxMotorTorque=250;
                        grinder2Joint.motor = grinder2Motor;
                        grinder2Joint.useMotor = true;
                        GameObject.Destroy(grinder2.GetComponent<SpriteRenderer>());

                        foreach (Collider2D c in Instance.GetComponents<Collider2D>())
                        {
                            Physics2D.IgnoreCollision(grinder1.GetComponent<Collider2D>(), c, true);
                            Physics2D.IgnoreCollision(grinder2.GetComponent<Collider2D>(), c, true);
                        }

                        //WHEELS
                        Vector2[] wps={new Vector2(-3.57f, -2f), new Vector2(-1.7f, -2f), new Vector2(4.6f, -1.9f)};

                        CarBehaviour car = Instance.AddComponent<CarBehaviour>();
                        car.WheelJoints = new WheelJoint2D[3];

                        GarbageTruck truck = Instance.AddComponent<GarbageTruck>();
                        truck.objects=new GameObject[7];
                        truck.wheels=new WheelJoint2D[3];

                        for(int i=0;i<3;i++)
                        {
                            GameObject wheel = GameObject.Instantiate(w, Instance.transform.position+new Vector3(wps[i].x*direction, wps[i].y, 0f), Quaternion.identity);
                            wheel.transform.localScale*=1.75f;
                            wheel.GetComponent<Rigidbody2D>().mass=150f;
                            wheel.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Top");
                            wheel.GetComponent<SpriteRenderer>().sortingOrder=1;
                            WheelJoint2D wj = Instance.AddComponent<WheelJoint2D>();
                            wj.connectedBody=wheel.GetComponent<Rigidbody2D>();
                            wj.anchor=wps[i];
                            wj.autoConfigureConnectedAnchor=true;
                            JointSuspension2D js = wj.suspension;
                            js.dampingRatio=0.75f;
                            js.frequency=5f;
                            wj.suspension=js;
                            wj.breakForce=15000f;
                            car.WheelJoints[i]=wj;
                            truck.wheels[i]=wj;
                            truck.objects[i]=wheel;

                            foreach (Collider2D c in back.GetComponents<Collider2D>())
                            {
                                Physics2D.IgnoreCollision(wheel.GetComponent<Collider2D>(), c, true);
                            }
                        }

                        //SMOKE
                        JointAngleLimits2D jointZero = new JointAngleLimits2D();
                        jointZero.min = 0; jointZero.max = 0;

                        HingeJoint2D smoke = GameObject.Instantiate(ModAPI.FindSpawnable("Particle Projector").Prefab, Instance.transform.position + new Vector3(4f*direction, 2f, 0f), Quaternion.identity).AddComponent<HingeJoint2D>();
                        smoke.connectedBody=Instance.GetComponent<Rigidbody2D>();
                        smoke.limits=jointZero;
                        smoke.GetComponent<SpriteRenderer>().sprite=null;
                        smoke.transform.parent = Instance.transform;

                        truck.objects[3]=back.gameObject;
                        truck.objects[4]=grinder1;
                        truck.objects[5]=grinder2;
                        truck.objects[6]=smoke.gameObject;


                        //CAR BEHAVIOUR
                        car.MotorSpeed=-1000;
                        car.Activated=false;
                        car.EngineStartup=ModAPI.LoadSound("start.mp3");
                        car.EngineShutoff=ModAPI.LoadSound("stop.mp3");
                        car.Phys=Instance.GetComponent<PhysicalBehaviour>();
                        car.IsBrakeEngaged=true;

                        truck.car=car;
                        truck.source=Instance.AddComponent<AudioSource>();//the car behaviour's loop property does not loop on its own
                        truck.source.clip=ModAPI.LoadSound("loop.mp3");
                        truck.source.loop=true;
                        truck.source.volume=0.5f;
                        truck.source.minDistance=0.1f;
                        truck.source.maxDistance=1f;
                    }
                }
            );
        }
    }

    public class GarbageTruckBack : MonoBehaviour
    {
        HingeJoint2D hinge;
        JointMotor2D jointMotor;

        public float direction;

        void Start()
        {
            hinge=GetComponent<HingeJoint2D>();
            jointMotor=hinge.motor;
            jointMotor.motorSpeed=-10f*direction;
            hinge.motor=jointMotor;

            gameObject.AddComponent<UseEventTrigger>().Action=this.ToggleOpen;
        }

        public void ToggleOpen()
        {
            jointMotor.motorSpeed*=-1f;
            hinge.motor=jointMotor;
        }
    }

    public class GarbageTruck : MonoBehaviour
    {
        public GameObject[] objects;

        public WheelJoint2D[] wheels;

        public CarBehaviour car;

        public AudioSource source;

        private bool playing = false;

        float speed=20f;

        void Update()
        {
            if(car!=null && car.Activated)
            {
                if(speed<150f){speed+=0.05f;}
                
                foreach(WheelJoint2D w in wheels)
                {
                    JointMotor2D jm = w.motor;
                    jm.maxMotorTorque=speed;
                    w.motor=jm;
                }
                
                if(!playing)
                {
                    source.Play();
                    playing=true;
                }
            }
            else
            {
                source.Stop();
                playing=false;
                speed=20f;
            }
        }
        
        void OnDestroy()
        {
            foreach(GameObject o in objects){GameObject.Destroy(o);}
        }
    }
}