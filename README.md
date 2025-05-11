# Introduction

The goal of this project is to simplify the implementation of the Inspector editor. By adding attributes in the code, non-serialized properties and methods can be displayed, with support for runtime changes and execution.

## Supported Types
- [x] Serializable Fields
- [x] Non-Serializable Fields
- [x] Properties
- [x] Readonly Properties
- [x] Methods (with parameters)
- [x] Non-Serializable Classes

## Usage Example

If I want to test if the Increase function works correctly, I can simply add [Drawable] to operate it directly in the Inspector (Sometimes, unit tests are not that convenient).

```C#
internal class Sample : MonoBehaviour
{
    [Drawable]
    private int _runtimeNumber;

    [Drawable]
    private void Increase(int number)
    {
        _runtimeNumber += number;
    }
}
```
<img src="https://github.com/user-attachments/assets/30af0781-4bd0-45f4-bcd8-4f2b72dd73a1" alt="sample" width="400"/>

# Installation

### Git URL

Project supports Unity Package Manager. To install the project as a Git package do the following:

1. In Unity, open **Window** -> **Package Manager**.
2. Press the **+** button, choose "**Add package from git URL...**"
3. Enter "https://github.com/anohis/DrawableMember.git#upm" and press **Add**.

