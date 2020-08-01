// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/input/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""cb089c3f-3f85-4a4a-8b03-71fb97cf4fd5"",
            ""actions"": [
                {
                    ""name"": ""movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""bf35e5c3-88fc-4c25-a7d3-2e486eb588ef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""jump"",
                    ""type"": ""Button"",
                    ""id"": ""d2b70c6c-b1e6-4746-90c1-5696be0a4ebd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""shoot"",
                    ""type"": ""Button"",
                    ""id"": ""374df6de-afc2-4a1d-90e1-7e77e00e7305"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""cancel"",
                    ""type"": ""Button"",
                    ""id"": ""73671514-ceb2-47a8-bba1-c4c5878d368c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""submit"",
                    ""type"": ""Button"",
                    ""id"": ""421bb05c-eb03-40e0-b1f0-3bd771b3a777"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""action"",
                    ""type"": ""Button"",
                    ""id"": ""a11e26b5-34ce-4e0b-a23a-56a35216b149"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""special"",
                    ""type"": ""Button"",
                    ""id"": ""6edd3a0b-66b3-488a-bc90-10a6d5ec97a9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""run"",
                    ""type"": ""Button"",
                    ""id"": ""a66eb25c-8369-4408-a038-ef50b168b899"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""261772c7-b67b-4f7a-b14c-b4ee67446cf7"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""fef73e8a-a452-4f5e-82e3-21a41da4e453"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""bbbe9d7a-76ff-411b-88fb-538bc5dca51d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""da568a3b-5d3b-4882-8d30-fc4e538473bc"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""45a5aa61-0904-4e40-a6f2-bbab89342d7d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""89baf106-f97e-4fdd-b912-a38c4f1c6c81"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a0ec396c-0a26-4af8-b3e7-da161da7be1f"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f7461f59-ca60-4120-90a7-93c192205973"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c64d7164-2689-4562-88b5-1bf8d9b4d828"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a13f13be-c5c6-43ff-9420-e4ebb5db7c60"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""gamepad"",
                    ""id"": ""defe5553-a4a3-4818-9037-28534782aca5"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c654337a-2149-45e5-a075-69ab65f5d1df"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""657ba474-07e2-4bfa-887b-fb9cd0f358a9"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ac875c97-b7e6-4494-8bcb-985c015a673c"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ff81b387-c816-47ff-ad5b-bdaad9c885f2"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""ps3controller"",
                    ""id"": ""0d22ce95-1442-4617-b2fb-781d6d65c8d6"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9e37388f-8b32-4a99-84af-b663a680d93b"",
                    ""path"": ""<Joystick>/stick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0e0648da-7ed9-4afb-9677-052680b7c2ce"",
                    ""path"": ""<Joystick>/stick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d27f9b05-7a7c-4476-8eb0-14bd058bdbb1"",
                    ""path"": ""<Joystick>/stick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""31d9de40-08af-4b11-ae2c-33ac13c5712c"",
                    ""path"": ""<Joystick>/stick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""d2c9a032-63d1-4b13-b7ac-ce1a14669d93"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d9bee0be-6f9a-47c6-9efe-f3466a3acfba"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""647f1f2b-9cd9-4095-8e32-4ed19ff1866f"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6b23ab2f-9cc7-4288-aab0-85c36bd9df9b"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""74572bc2-eee0-4735-bb4e-6635bc2a7233"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4baba7f1-4f44-4515-84f1-86f45d7cc0f5"",
                    ""path"": ""<Keyboard>/rightCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""11715e0f-f18b-4287-b035-9600fa835721"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d89cc26f-3e73-4462-b8d4-ad5e004b8a98"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""53ee70ca-a722-4040-8dc3-ab9a6bb223e4"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""db7f5c47-3050-44e9-8d8f-e3cef86d2d6f"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce64a8b5-8843-45a4-afc0-c6b78ab4a4df"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fe863276-85a0-4b6e-860a-3394e1545b82"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03fa9b5e-7796-4631-8c12-de669abeb41e"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4bf773d9-9d64-41b9-b229-a0ccd25f9b96"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4b97450-bfff-4a70-9fac-4d70d928d9a5"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button10"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""50d64718-7252-4eac-aac8-22ca38f6c427"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a7a2a8c2-59a6-486d-8cd0-32393a77f772"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8a2db1c3-60d7-4e29-a257-06a9a21f5c13"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9072ad66-5d48-43b5-9885-f113545807de"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/trigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""348078d7-8409-445d-a155-ec065c950533"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""special"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""90f75848-37c9-4349-9feb-26834bbdc8a7"",
                    ""path"": ""<Mouse>/forwardButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""special"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b6a444d-0f38-4537-8de4-8f415bb511e6"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""special"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6856005c-1e06-4b0e-bb4d-aff8d9faa4f7"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""special"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bdbc1388-3d92-42e6-90bd-732a17b1f8bf"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7aed4ed-bac5-4af5-8f86-5914ca224614"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0bd8185f-d3ac-4846-9420-0af46b913d8e"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button11"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UINavigation"",
            ""id"": ""6b0c0351-7dee-44fe-b578-6bed4767bf21"",
            ""actions"": [
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""1821e80e-16a3-4ed7-959f-c10d5f053b76"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""ba525dcf-7691-4699-8928-f545cdf86204"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""4c85ec8e-f0da-434a-a613-18223f79511a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""5210938e-a4d2-496d-b457-f9085e306aca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""7af8cee3-45ef-4337-b8a6-e879fcb336fe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""cca3c20c-43ce-41ec-a741-ea17dbec30ec"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0060e9d7-b8a8-4677-9586-dabe5521d230"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""97494625-14db-4ca4-97fc-90605c0b0f8c"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1c9a559-338c-4752-99fb-15786d3b7a70"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6678a89-b4b0-44e2-a83e-4699b26b435a"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ca2b3e7a-f83f-4b73-8fc0-c1bcacf24868"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/stick/up"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b9ba2b99-a4d3-451e-84c0-4c94f9d30610"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button13"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1aa192e1-b5ec-4bdc-96fd-c3d0a2e642ce"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""60a7dfda-5278-4820-878b-747657f44b87"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""728aa15d-4fae-439f-9825-692b453b9509"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b06b2cac-ae83-472c-b9d3-db1b42b12c21"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2976239d-11cb-4ccb-bc51-cd182137e4af"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/stick/down"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f380e8f2-1d23-4423-ace9-f6027df78b07"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button15"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85529c4e-80bf-4633-8694-f577b70434de"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2db9ba47-239d-4e9f-8912-46efb60d9935"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""caf319c7-87a2-4c5c-a66c-76ece87f66da"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e478b4cb-5a61-488f-ad34-94527308aea4"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b4f42465-71ca-4362-9277-78d55b4cdf64"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/stick/left"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e6cc1aaa-a585-465b-9a64-cf3156293ef5"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button16"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""94b83ac6-dc27-4d6c-b3e3-d20e839d66bd"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5e6c18d1-2958-409f-adf5-56d5e5cf4d79"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8346877f-f65e-48a5-b87f-1f6535d0d196"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1965f6b8-e510-4b28-96cb-5457c3bf88f5"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""59aaab0c-b349-4350-89de-91867d4fc25d"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/stick/right"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7eefa87b-be52-4b6c-be88-86c17b21d0ce"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button14"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f5e91b2d-52b2-4058-abd2-cd81f624e9a2"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""abfaaee3-1729-41b9-8afe-9e715a369503"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec125c74-0ce0-482e-9097-8380cfe39ede"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""052d9ff2-177c-457c-be84-b09084670393"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""428e1f4c-a6a9-4964-a666-7e022296f1a3"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aeeee366-bea9-41c5-bb17-e06f7424f578"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button10"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b9ecb624-910b-456d-9613-a758f72dfc25"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e2ad1c17-f9a6-4e8e-8cf5-6ce779c6d239"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6439f8c3-095f-46c4-8bd7-2246f449f240"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f407e39e-9523-42ad-bfb7-4d754021d16a"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Other"",
            ""id"": ""e05ddf2f-d6ab-40d9-a85e-1a650f4515d5"",
            ""actions"": [
                {
                    ""name"": ""change"",
                    ""type"": ""Button"",
                    ""id"": ""af87e66c-aa45-4732-97b7-ff51177d9082"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""toggle_run_keyboard"",
                    ""type"": ""Button"",
                    ""id"": ""e746bb66-629b-4448-8cc6-26db1542228f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""toggle_stats_keyboard"",
                    ""type"": ""Button"",
                    ""id"": ""1e9f2494-091a-484a-9337-0b099be1a935"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""toggle_camera_keyboard"",
                    ""type"": ""Button"",
                    ""id"": ""7007229e-e676-4c68-8ae0-fce1fc835d33"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""25aafa3a-750d-4f03-98d6-fd849b83d472"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""toggle_run_keyboard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8970146c-300d-4fc3-b16d-855b320750e4"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""toggle_run_keyboard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d87c4e3-2beb-4870-b592-f9308220b71f"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button16"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""toggle_run_keyboard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b265e8d-6f24-46fa-948b-39b42a8b3d12"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""toggle_stats_keyboard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0a42432a-b926-4bb0-ab3d-d4c4e800a67a"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""toggle_stats_keyboard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa0174ef-4ad0-49fe-b2f2-5e8b2ab8b332"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button13"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""toggle_stats_keyboard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""76d75319-7760-44f0-91ad-e92246b2295e"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""change"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3b412c3e-0377-48bf-b6bf-b1c2c00e6384"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""change"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""73d9fa21-d00c-432d-846d-c103ea20dee2"",
                    ""path"": ""<Keyboard>/rightShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""change"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""77d7cc0c-3409-45fe-94d0-c1060ef44195"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button12"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""change"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""08d05f50-e234-4837-a69b-9ea65680546f"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""toggle_camera_keyboard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be0668f0-6c2d-45e6-ba98-07ddbe1905e9"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""toggle_camera_keyboard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6fc94583-3f59-42f5-8d6a-b2336ec799cf"",
                    ""path"": ""<HID::Sony PLAYSTATION(R)3 Controller>/button14"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""toggle_camera_keyboard"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_movement = m_Player.FindAction("movement", throwIfNotFound: true);
        m_Player_jump = m_Player.FindAction("jump", throwIfNotFound: true);
        m_Player_shoot = m_Player.FindAction("shoot", throwIfNotFound: true);
        m_Player_cancel = m_Player.FindAction("cancel", throwIfNotFound: true);
        m_Player_submit = m_Player.FindAction("submit", throwIfNotFound: true);
        m_Player_action = m_Player.FindAction("action", throwIfNotFound: true);
        m_Player_special = m_Player.FindAction("special", throwIfNotFound: true);
        m_Player_run = m_Player.FindAction("run", throwIfNotFound: true);
        // UINavigation
        m_UINavigation = asset.FindActionMap("UINavigation", throwIfNotFound: true);
        m_UINavigation_Up = m_UINavigation.FindAction("Up", throwIfNotFound: true);
        m_UINavigation_Down = m_UINavigation.FindAction("Down", throwIfNotFound: true);
        m_UINavigation_Left = m_UINavigation.FindAction("Left", throwIfNotFound: true);
        m_UINavigation_Right = m_UINavigation.FindAction("Right", throwIfNotFound: true);
        m_UINavigation_Submit = m_UINavigation.FindAction("Submit", throwIfNotFound: true);
        m_UINavigation_Cancel = m_UINavigation.FindAction("Cancel", throwIfNotFound: true);
        // Other
        m_Other = asset.FindActionMap("Other", throwIfNotFound: true);
        m_Other_change = m_Other.FindAction("change", throwIfNotFound: true);
        m_Other_toggle_run_keyboard = m_Other.FindAction("toggle_run_keyboard", throwIfNotFound: true);
        m_Other_toggle_stats_keyboard = m_Other.FindAction("toggle_stats_keyboard", throwIfNotFound: true);
        m_Other_toggle_camera_keyboard = m_Other.FindAction("toggle_camera_keyboard", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_movement;
    private readonly InputAction m_Player_jump;
    private readonly InputAction m_Player_shoot;
    private readonly InputAction m_Player_cancel;
    private readonly InputAction m_Player_submit;
    private readonly InputAction m_Player_action;
    private readonly InputAction m_Player_special;
    private readonly InputAction m_Player_run;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @movement => m_Wrapper.m_Player_movement;
        public InputAction @jump => m_Wrapper.m_Player_jump;
        public InputAction @shoot => m_Wrapper.m_Player_shoot;
        public InputAction @cancel => m_Wrapper.m_Player_cancel;
        public InputAction @submit => m_Wrapper.m_Player_submit;
        public InputAction @action => m_Wrapper.m_Player_action;
        public InputAction @special => m_Wrapper.m_Player_special;
        public InputAction @run => m_Wrapper.m_Player_run;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @shoot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @shoot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @shoot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @cancel.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                @cancel.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                @cancel.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCancel;
                @submit.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubmit;
                @submit.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubmit;
                @submit.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSubmit;
                @action.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction;
                @action.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction;
                @action.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAction;
                @special.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecial;
                @special.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecial;
                @special.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSpecial;
                @run.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRun;
                @run.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRun;
                @run.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRun;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @movement.started += instance.OnMovement;
                @movement.performed += instance.OnMovement;
                @movement.canceled += instance.OnMovement;
                @jump.started += instance.OnJump;
                @jump.performed += instance.OnJump;
                @jump.canceled += instance.OnJump;
                @shoot.started += instance.OnShoot;
                @shoot.performed += instance.OnShoot;
                @shoot.canceled += instance.OnShoot;
                @cancel.started += instance.OnCancel;
                @cancel.performed += instance.OnCancel;
                @cancel.canceled += instance.OnCancel;
                @submit.started += instance.OnSubmit;
                @submit.performed += instance.OnSubmit;
                @submit.canceled += instance.OnSubmit;
                @action.started += instance.OnAction;
                @action.performed += instance.OnAction;
                @action.canceled += instance.OnAction;
                @special.started += instance.OnSpecial;
                @special.performed += instance.OnSpecial;
                @special.canceled += instance.OnSpecial;
                @run.started += instance.OnRun;
                @run.performed += instance.OnRun;
                @run.canceled += instance.OnRun;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UINavigation
    private readonly InputActionMap m_UINavigation;
    private IUINavigationActions m_UINavigationActionsCallbackInterface;
    private readonly InputAction m_UINavigation_Up;
    private readonly InputAction m_UINavigation_Down;
    private readonly InputAction m_UINavigation_Left;
    private readonly InputAction m_UINavigation_Right;
    private readonly InputAction m_UINavigation_Submit;
    private readonly InputAction m_UINavigation_Cancel;
    public struct UINavigationActions
    {
        private @PlayerControls m_Wrapper;
        public UINavigationActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Up => m_Wrapper.m_UINavigation_Up;
        public InputAction @Down => m_Wrapper.m_UINavigation_Down;
        public InputAction @Left => m_Wrapper.m_UINavigation_Left;
        public InputAction @Right => m_Wrapper.m_UINavigation_Right;
        public InputAction @Submit => m_Wrapper.m_UINavigation_Submit;
        public InputAction @Cancel => m_Wrapper.m_UINavigation_Cancel;
        public InputActionMap Get() { return m_Wrapper.m_UINavigation; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UINavigationActions set) { return set.Get(); }
        public void SetCallbacks(IUINavigationActions instance)
        {
            if (m_Wrapper.m_UINavigationActionsCallbackInterface != null)
            {
                @Up.started -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnUp;
                @Up.performed -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnUp;
                @Up.canceled -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnUp;
                @Down.started -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnDown;
                @Left.started -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnLeft;
                @Right.started -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnRight;
                @Submit.started -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnSubmit;
                @Submit.performed -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnSubmit;
                @Submit.canceled -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnSubmit;
                @Cancel.started -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_UINavigationActionsCallbackInterface.OnCancel;
            }
            m_Wrapper.m_UINavigationActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Up.started += instance.OnUp;
                @Up.performed += instance.OnUp;
                @Up.canceled += instance.OnUp;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
                @Submit.started += instance.OnSubmit;
                @Submit.performed += instance.OnSubmit;
                @Submit.canceled += instance.OnSubmit;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
            }
        }
    }
    public UINavigationActions @UINavigation => new UINavigationActions(this);

    // Other
    private readonly InputActionMap m_Other;
    private IOtherActions m_OtherActionsCallbackInterface;
    private readonly InputAction m_Other_change;
    private readonly InputAction m_Other_toggle_run_keyboard;
    private readonly InputAction m_Other_toggle_stats_keyboard;
    private readonly InputAction m_Other_toggle_camera_keyboard;
    public struct OtherActions
    {
        private @PlayerControls m_Wrapper;
        public OtherActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @change => m_Wrapper.m_Other_change;
        public InputAction @toggle_run_keyboard => m_Wrapper.m_Other_toggle_run_keyboard;
        public InputAction @toggle_stats_keyboard => m_Wrapper.m_Other_toggle_stats_keyboard;
        public InputAction @toggle_camera_keyboard => m_Wrapper.m_Other_toggle_camera_keyboard;
        public InputActionMap Get() { return m_Wrapper.m_Other; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(OtherActions set) { return set.Get(); }
        public void SetCallbacks(IOtherActions instance)
        {
            if (m_Wrapper.m_OtherActionsCallbackInterface != null)
            {
                @change.started -= m_Wrapper.m_OtherActionsCallbackInterface.OnChange;
                @change.performed -= m_Wrapper.m_OtherActionsCallbackInterface.OnChange;
                @change.canceled -= m_Wrapper.m_OtherActionsCallbackInterface.OnChange;
                @toggle_run_keyboard.started -= m_Wrapper.m_OtherActionsCallbackInterface.OnToggle_run_keyboard;
                @toggle_run_keyboard.performed -= m_Wrapper.m_OtherActionsCallbackInterface.OnToggle_run_keyboard;
                @toggle_run_keyboard.canceled -= m_Wrapper.m_OtherActionsCallbackInterface.OnToggle_run_keyboard;
                @toggle_stats_keyboard.started -= m_Wrapper.m_OtherActionsCallbackInterface.OnToggle_stats_keyboard;
                @toggle_stats_keyboard.performed -= m_Wrapper.m_OtherActionsCallbackInterface.OnToggle_stats_keyboard;
                @toggle_stats_keyboard.canceled -= m_Wrapper.m_OtherActionsCallbackInterface.OnToggle_stats_keyboard;
                @toggle_camera_keyboard.started -= m_Wrapper.m_OtherActionsCallbackInterface.OnToggle_camera_keyboard;
                @toggle_camera_keyboard.performed -= m_Wrapper.m_OtherActionsCallbackInterface.OnToggle_camera_keyboard;
                @toggle_camera_keyboard.canceled -= m_Wrapper.m_OtherActionsCallbackInterface.OnToggle_camera_keyboard;
            }
            m_Wrapper.m_OtherActionsCallbackInterface = instance;
            if (instance != null)
            {
                @change.started += instance.OnChange;
                @change.performed += instance.OnChange;
                @change.canceled += instance.OnChange;
                @toggle_run_keyboard.started += instance.OnToggle_run_keyboard;
                @toggle_run_keyboard.performed += instance.OnToggle_run_keyboard;
                @toggle_run_keyboard.canceled += instance.OnToggle_run_keyboard;
                @toggle_stats_keyboard.started += instance.OnToggle_stats_keyboard;
                @toggle_stats_keyboard.performed += instance.OnToggle_stats_keyboard;
                @toggle_stats_keyboard.canceled += instance.OnToggle_stats_keyboard;
                @toggle_camera_keyboard.started += instance.OnToggle_camera_keyboard;
                @toggle_camera_keyboard.performed += instance.OnToggle_camera_keyboard;
                @toggle_camera_keyboard.canceled += instance.OnToggle_camera_keyboard;
            }
        }
    }
    public OtherActions @Other => new OtherActions(this);
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnAction(InputAction.CallbackContext context);
        void OnSpecial(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
    }
    public interface IUINavigationActions
    {
        void OnUp(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
        void OnLeft(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
    }
    public interface IOtherActions
    {
        void OnChange(InputAction.CallbackContext context);
        void OnToggle_run_keyboard(InputAction.CallbackContext context);
        void OnToggle_stats_keyboard(InputAction.CallbackContext context);
        void OnToggle_camera_keyboard(InputAction.CallbackContext context);
    }
}
