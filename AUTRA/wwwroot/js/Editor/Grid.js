function Grid(spaceX, spaceZ, numberInX, numberInZ,shift) {
    //the line extention from intersection
    // let shift = 0.05 * (spaceX[spaceX.length - 1] + spaceZ[spaceX.length - 1]);

    
    //spacing in x-direction
    this.spaceX = spaceX;
    //spacing in z-direction
    this.spaceZ = spaceZ;
    //No of grids in x-direction
    this.numberInX = numberInX;
    //No of grids in z-direction
    this.numberInZ = numberInZ;

    //Variable spacing:
    this.lineLengthInX = spaceX[spaceX.length - 1] + 2 * shift;
    this.lineLengthInZ = spaceZ[spaceZ.length - 1] + 2 * shift;

    //Constant spacing
    // this.lineLengthInX = spaceX * (numberInX - 1) + 2 * shift;
    // this.lineLengthInZ = spaceZ * (numberInZ - 1) + 2 * shift;

    //Create a Group of grid lines
    this.linesInX = new THREE.Group();
    this.linesInZ = new THREE.Group();

    // let accumlateSpaceX = 0;
    // let accumlateSpaceZ = 0;

    //Fill the horizontal Group 
    for (let i = 0; i < numberInX; i++) {
        // accumlateSpaceX += spaceX[i];
        this.linesInX.add(new Line(new THREE.Vector3(spaceX[i], 0, -shift), new THREE.Vector3(0, 0, 1), this.lineLengthInZ).line);
    }
    //scene.add(this.linesInX); //Add line Group to the scene

    //Fill the vertical Group 
    for (let i = 0; i < numberInZ; i++) {
        // accumlateSpaceZ += spaceZ[i];
        this.linesInZ.add((new Line(new THREE.Vector3(-shift, 0, spaceZ[i]), new THREE.Vector3(1, 0, 0), this.lineLengthInX)).line);
    }
    //scene.add(this.linesInZ); //Add line Group to the scene
}

function Line(startPoint, direction, length) {
    this.startPoint = startPoint || new THREE.Vector3(0, 0, 0);
    this.direction = direction || new THREE.Vector3(1, 0, 0);
    this.length = length || 1000;
    this.endPoint = new THREE.Vector3(this.startPoint.x + this.length * this.direction.x, this.startPoint.y + this.length * this.direction.y, this.startPoint.z + this.length * this.direction.z);
    this.material = new THREE.LineDashedMaterial({
        color: 0x333333,
        opacity: 0.3,
        transparent: true,
        gapSize: 0.5,
        dashSize: 0.5,
        scale: 1
    });
    this.geometry = new THREE.BufferGeometry().setFromPoints([this.startPoint, this.endPoint]);
    this.line = new THREE.Line(this.geometry, this.material);
    this.line.computeLineDistances();
}

function sum(a, b) {
    return a + b;
}