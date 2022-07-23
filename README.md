# Serializable Interface

<p align="center">
	<img alt="GitHub package.json version" src ="https://img.shields.io/github/package-json/v/Thundernerd/Unity3D-SerializableInterface" />
	<a href="https://github.com/Thundernerd/Unity3D-SerializableInterface/issues">
		<img alt="GitHub issues" src ="https://img.shields.io/github/issues/Thundernerd/Unity3D-SerializableInterface" />
	</a>
	<a href="https://github.com/Thundernerd/Unity3D-SerializableInterface/pulls">
		<img alt="GitHub pull requests" src ="https://img.shields.io/github/issues-pr/Thundernerd/Unity3D-SerializableInterface" />
	</a>
	<a href="https://github.com/Thundernerd/Unity3D-SerializableInterface/blob/master/LICENSE.md">
		<img alt="GitHub license" src ="https://img.shields.io/github/license/Thundernerd/Unity3D-SerializableInterface" />
	</a>
	<img alt="GitHub last commit" src ="https://img.shields.io/github/last-commit/Thundernerd/Unity3D-SerializableInterface" />
</p>

A wrapper that allows you to serialize interfaces. Both UnityEngine.Object and regular object implementers work!

## Installation
1. The package is available on the [openupm registry](https://openupm.com). You can install it via [openupm-cli](https://github.com/openupm/openupm-cli).
```
openupm add net.tnrd.serializableinterface
```

2. Installing through a [Unity Package](http://package-installer.glitch.me/v1/installer/package.openupm.com/net.tnrd.serializableinterface?registry=https://package.openupm.com) created by the [Package Installer Creator](https://package-installer.glitch.me) from [Needle](https://needle.tools)

[<img src="https://img.shields.io/badge/-Download-success?style=for-the-badge"/>](http://package-installer.glitch.me/v1/installer/package.openupm.com/net.tnrd.serializableinterface?registry=https://package.openupm.com)

## Usage

Usage is pretty easy and straightforward. Assuming you have the following interface
```c#
public interface IMyInterface
{
    void Greet();
}
```

You can add it to a behaviour like so
```c#
using TNRD;
using UnityEngine;

public class MyBehaviour : MonoBehaviour
{
    [SerializeField] private SerializableInterface<IMyInterface> mySerializableInterface;

    private void Awake()
    {
        mySerializableInterface.Value.Greet();
    }
}
```

Back in the Unity inspector it will look like this

![image](https://user-images.githubusercontent.com/5531467/164994596-82ce84c8-27bc-4957-a297-f7c7d69c79d9.png)

And when you click on the object selector button you will be shown a dropdown window with all possible options like this

![image](https://user-images.githubusercontent.com/5531467/164994604-15a0d060-72d1-440b-926b-883dd5f31955.png)

As you can see you can select items from multiple locations:
- Assets (Scriptable Objects and Prefabs that implement said interface)
- Classes (custom classes that implement said interface)
- Scene (objects in the scene that implement said interface)

For the sake of example for the image above I have created some simple implementations

```c#
using UnityEngine;

public class MyComponent : MonoBehaviour, IMyInterface
{
    /// <inheritdoc />
    public void Greet()
    {
        Debug.Log("Hello, World! I'm MyComponent");
    }
}
```

```c#
using UnityEngine;

public class MyPoco : IMyInterface
{
    /// <inheritdoc />
    public void Greet()
    {
        Debug.Log("Hello, World! I'm MyPoco");
    }
}
```

```c#
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable")]
public class MyScriptable : ScriptableObject, IMyInterface
{
    /// <inheritdoc />
    public void Greet()
    {
        Debug.Log("Hello, World! I'm MyScriptable");
    }
}
```


## Support
**Serializable Interface** is a small and open-source utility that I hope helps other people. It is by no means necessary but if you feel generous you can support me by donating.

[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/J3J11GEYY)

## Contributions
Pull requests are welcomed. Please feel free to fix any issues you find, or add new features.

