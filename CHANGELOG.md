### [1.12.1](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.12.0...v1.12.1) (2022-09-10)


### Bug Fixes

* fix editor drawing of managed classes inside list ([8d66349](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/8d663496eef8eab34e5553617ad9697d3e61d8f1))
* fix issue with unity version 2021.1 ([d81e9df](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/d81e9df6fbc4fb0eb2ca70c7b3a46418045923aa)), closes [Thundernerd/Unity3D-SerializableInterface#51](https://github.com/Thundernerd/Unity3D-SerializableInterface/issues/51)
* fix version directive in IconUtility ([ffe848c](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/ffe848c940898c22816268d8be2aa3d84721a678)), closes [Thundernerd/Unity3D-SerializableInterface#51](https://github.com/Thundernerd/Unity3D-SerializableInterface/issues/51)
* make changes requested in code review. Also, fix stop condition in for loop that traverses serialized property's path in SerializedPropertyUtilities.GetValue() function ([191d442](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/191d442026fbc37fb86cc980bce68bfb5e8fb874))

## [1.12.0](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.11.1...v1.12.0) (2022-09-09)


### Features

* added GetValue method for accurately getting a serialized property's value ([96c08c8](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/96c08c88ec3e5f76e34067663ad20b80cd6b6c0f))

### [1.11.1](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.11.0...v1.11.1) (2022-08-02)


### Bug Fixes

* removed custom line drawing to look more like Unity native ([65b7329](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/65b73293c6f7212db578bb1e1313ea1789abc8d0))

## [1.11.0](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.10.0...v1.11.0) (2022-08-02)


### Features

* added methods for getting value combined with null checks ([1154dbb](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/1154dbb8f30476daf62fdbe445dfa520615bef5a))

## [1.10.0](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.9.0...v1.10.0) (2022-07-25)


### Features

* added methods for getting and setting values with serialized properties ([aba0c30](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/aba0c305b87ab843a3842d584a38d58487e61a89))
* added passing serialized property to CustomObjectDrawer and Dropdown ([5913ab7](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/5913ab76e7759e2264a2979081972468dcae2f40))
* added serialized property extensions for convenience ([547c892](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/547c89271c892ca1bfa14e30958437c379dc9853))

### [1.9.0](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.8.0...v1.9.0) (2202-07-23)


### What's Changed
* Added RawReferenceDrawer.Styles.cs + DrawLine - #41 by @marc-antoine-girard in https://github.com/Thundernerd/Unity3D-SerializableInterface/pull/42
* ReferenceMode + AvoidDuplicateReferencesInArray by @marc-antoine-girard in https://github.com/Thundernerd/Unity3D-SerializableInterface/pull/43

### [1.8.0](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.7.3...v1.8.0) (2022-07-22)

### What's Changed
* fix: added null check to prevent null reference exception when clicking properties by @marc-antoine-girard in https://github.com/Thundernerd/Unity3D-SerializableInterface/pull/23
* #24 add none option custom drawers by @marc-antoine-girard in https://github.com/Thundernerd/Unity3D-SerializableInterface/pull/26
* Fixed Drag and Drop behaviour by @marc-antoine-girard in https://github.com/Thundernerd/Unity3D-SerializableInterface/pull/28
* Fixes #25 SerializableInterface maintains instances when set by Inspector by @marc-antoine-girard in https://github.com/Thundernerd/Unity3D-SerializableInterface/pull/27
* #34 add prefabs assets item builder by @marc-antoine-girard in https://github.com/Thundernerd/Unity3D-SerializableInterface/pull/35

### [1.7.3](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.7.2...v1.7.3) (2022-07-22)


### Refactors

* property drawer now reuses cached reference drawers ([f930d8e](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/f930d8ed92b8358e417d075f9a089f7161cadc50))

### [1.7.2](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.7.1...v1.7.2) (2022-07-22)


### Bug Fixes

* updated actions/checkout to version 3 ([ef748e1](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/ef748e176b864cd3dfc00f3b96e9cedb8783055d))

### [1.7.1](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.7.0...v1.7.1) (2022-06-18)


### Bug Fixes

* added package-lock.json to gitignore ([9fb467c](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/9fb467c605413529373a9e2a424762744fba1b1c))
* changed block scalar from literal to folded ([44b269f](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/44b269fa140c1b3a89fba78acb424d8a1609a0b5))

## [1.7.0](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.6.1...v1.7.0) (2022-06-12)


### Features

* added support for collections ([068b76c](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/068b76cb5072541b4c87a6c5e55669ee69e172be))


### Bug Fixes

* invalid scenes get skipped ([b13c42d](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/b13c42d2bcab44bacc71b71a3be3e8f9d40a1e5c))
* package-lock.json meta issue ([c23b9bc](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/c23b9bc68147acaf165b174a0206ac9cf43b3e6b))

### [1.6.1](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.6.0...v1.6.1) (2022-05-07)


### Bug Fixes

* add package-lock.json to package ([0b08f32](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/0b08f3251a72b21f61c8df2a4c7ffc0082f5d3d1))

## [1.6.0](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.5.2...v1.6.0) (2022-05-02)


### Features

* added drag and drop for unity reference ([ef45ebc](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/ef45ebccc6cd6d3a9a6902937ddc1aa3143b27e9))
* added raw reference drag and drop and combined them in base ([62f61e8](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/62f61e82389bcf97b776ef22cc6c591ef9920ce5))


### Bug Fixes

* removed unused namespaces ([0b1185c](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/0b1185c272d7284b26cfcd44295131f5c8a2741a))

### [1.5.2](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.5.1...v1.5.2) (2022-04-28)


### Bug Fixes

* updated namespaces ([c9acaf2](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/c9acaf2d044f862bf5fdcd0819215d37afc01090))

### [1.5.1](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.5.0...v1.5.1) (2022-04-28)


### Bug Fixes

* added newline at end of files ([a0d6575](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/a0d6575ef00721d6d3e5b2cb10c2836d5f4a2e03))

## [1.5.0](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.4.1...v1.5.0) (2022-04-28)


### Features

* added CustomObjectDrawer ([91e0beb](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/91e0beb5fd0e667c2f62cb10c1a8dd6192848040))
* added IReferenceDrawer ([fe32120](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/fe321208d7de803883b660ae1d0087a2c92569cf))
* added RawReferenceDrawer and UnityReferenceDrawer ([9e46ddc](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/9e46ddc1de6ba872233798e44cd8353d1451999d))


### Refactors

* moved drawing code to separate classes ([81cfffc](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/81cfffc4f3f46a273ba29bebd7beef15273a3782))

### [1.4.1](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.4.0...v1.4.1) (2022-04-28)


### Bug Fixes

* added missing meta file ([6fd976f](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/6fd976f46d7706f3daacf7b3982f64312f9208c2))

## [1.4.0](https://github.com/Thundernerd/Unity3D-SerializableInterface/compare/v1.3.0...v1.4.0) (2022-04-27)


### Features

* added semantic release ([74a928c](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/74a928c2818cf04056eaa2a44be386414477ffe5))
* added semantic-release configuration to package.json ([95e314b](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/95e314bb4c5b784ae10e07a1d2a9a5681421da67))
* added workflow ([d369a92](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/d369a92f8b04d740cb90622974e6b295c9ff50b8))


### Bug Fixes

* moved dependencies to devDependencies ([8441cc8](https://github.com/Thundernerd/Unity3D-SerializableInterface/commit/8441cc8a7c1daf1c00e7c4e379f616b4dace17fa))
