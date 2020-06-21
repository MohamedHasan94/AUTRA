function Grid(coordX, coordZ, shift, levels) {
    //coordinates in x-direction
    this.coordX = coordX;
    //coordinates in z-direction
    this.coordZ = coordZ;
    //levels
    this.levels = levels;

    this.cxs = [0];
    this.cys = [0];

    let numberInX = this.coordX.length;
    let numberInZ = this.coordZ.length;

    for (let i = 1; i < numberInX; i++) {
        this.cxs[i] = coordX[i] - coordX[i - 1];
    }

    for (let i = 1; i < numberInZ; i++) {
        this.cys[i] = coordZ[i] - coordZ[i - 1];
    }
    this.xLength = coordX[numberInX - 1];
    this.zLength = coordZ[numberInZ - 1];    

    //#region Creating Grids
    //#region Grid Lines
    let dashedMaterial = new THREE.LineDashedMaterial({
        color: 0x333333,
        opacity: 0.3,
        transparent: true,
        gapSize: 0.5,
        dashSize: 0.5,
        scale: 1
    });
    //Fill the vertical Group
    let gridGeometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(coordX[0], 0, -shift),
        new THREE.Vector3(coordX[0], 0, this.zLength + shift)]);

    for (let i = 1; i < numberInX; i++) {
        let geometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(coordX[i], 0, -shift),
            new THREE.Vector3(coordX[i], 0, this.zLength + shift)]);
        gridGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([gridGeometry, geometry]);
    }
    //Fill the horizontal Group
    for (let i = 0; i < numberInZ; i++) {
        let geometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(-shift, 0, coordZ[i]),
        new THREE.Vector3(this.xLength + shift, 0, coordZ[i])]);
        gridGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([gridGeometry, geometry]);
    }

    this.gridLines = new THREE.LineSegments(gridGeometry, dashedMaterial);
    this.gridLines.computeLineDistances();
    //#endregion
    //Grids Names
    let matrix = new THREE.Matrix4();
    let geoProperties = {
        font: myFont,
        size: 0.75,
        height: 0,
        curveSegments: 3,
        bevelEnabled: false
    };

    let gridNamesGeometry = new THREE.TextBufferGeometry(``, geoProperties); //Empty geometry to append geometries in
    for (let i = 0; i < numberInX; i++) { //Vertical grids (letters)
        let geometry = new THREE.TextBufferGeometry(`${String.fromCharCode(i + 65)}`, geoProperties);
        let geometry2 = geometry.clone();
        geometry.applyMatrix4(matrix.makeRotationX(-Math.PI / 2)).applyMatrix4(matrix.makeTranslation(coordX[i], 0, -shift - geoProperties.size));
        gridNamesGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([gridNamesGeometry, geometry]);

        geometry2.applyMatrix4(matrix.makeRotationX(-Math.PI / 2)).applyMatrix4(matrix.makeTranslation(coordX[i], 0, this.zLength + shift + geoProperties.size));
        gridNamesGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([gridNamesGeometry, geometry2]);
    }

    for (let i = 0; i < numberInZ; i++) { //Horizontal grids (Numbers)
        let geometry = new THREE.TextBufferGeometry(`${i + 1}`, geoProperties);
        let geometry2 = geometry.clone();
        geometry.applyMatrix4(matrix.makeRotationX(-Math.PI / 2)).applyMatrix4(matrix.makeTranslation(-shift - 1, 0, coordZ[i]));
        gridNamesGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([gridNamesGeometry, geometry]);

        geometry2.applyMatrix4(matrix.makeRotationX(-Math.PI / 2)).applyMatrix4(matrix.makeTranslation(this.xLength + shift, 0, coordZ[i]));
        gridNamesGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([gridNamesGeometry, geometry2]);
    }
    let textMaterial = new THREE.MeshBasicMaterial({ color: 0x333333, opacity: 0.3, transparent: true });
    this.gridNames = new THREE.Mesh(gridNamesGeometry, textMaterial);
    //#endregion

    //#region Creating Axess
    /*this.axes = new THREE.Mesh();
    this.axes.add(new THREE.ArrowHelper(xDirection, new THREE.Vector3(), this.lineLengthInX, 0xff0000, 0.05 * this.lineLengthInX));//x-Axis
    this.axes.add(new THREE.ArrowHelper(zDirection, new THREE.Vector3(), this.lineLengthInZ, 0x00ff00, 0.05 * this.lineLengthInZ));//y-Axis(In UI)
    this.axes.add(new THREE.ArrowHelper(new THREE.Vector3(0, 1, 0), new THREE.Vector3(), height + shift, 0x0000ff, 0.05 * (height + shift)));//z-Axis(In UI)
    */
    this.axes = new THREE.AxesHelper(shift);
    //let namesGeometry = new THREE.TextBufferGeometry('', geoProp);

    let namesGeometry = new THREE.TextBufferGeometry(`X`, geoProperties);
    namesGeometry.applyMatrix4(matrix.makeTranslation(shift, 0, 0));
    //namesGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([namesGeometry, xGeometry]);

    let yGeometry = new THREE.TextBufferGeometry(`Y`, geoProperties);
    yGeometry.applyMatrix4(matrix.makeTranslation(0, 0, shift));
    namesGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([namesGeometry, yGeometry]);

    let zGeometry = new THREE.TextBufferGeometry(`Z`, geoProperties);
    zGeometry.applyMatrix4(matrix.makeTranslation(0, shift, 0));
    namesGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([namesGeometry, zGeometry]);

    let lettersMesh = new THREE.Mesh(namesGeometry, textMaterial);
    this.axes.add(lettersMesh);

    this.axes.position.set(-shift * 1.5, 0, -shift * 1.5);
    //this.axes.position.set(-35, -15, -60);
    //#endregion

    //#region Create Dimensions
    let offset = shift / 3;
    this.dimensions = new THREE.Group();
    let dimMaterial = new THREE.LineBasicMaterial({ color: 0x0000ff });

    //Inner z-dimension line
    let dimLineGeometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(this.xLength + offset, 0, 0), new THREE.Vector3(this.xLength + offset, 0, this.zLength)]);

    //Toatl z-dimension line
    let dimLineGeometry1 = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(this.xLength + 2 * offset, 0, 0), new THREE.Vector3(this.xLength + 2 * offset, 0, this.zLength)]);
    dimLineGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([dimLineGeometry, dimLineGeometry1]);

    let sideGeometry;
    for (let i = 0; i < numberInX; i++) {//Side lines of inner x-dimensions
        sideGeometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(coordX[i], 0, this.zLength + offset - 0.3), new THREE.Vector3(coordX[i], 0, this.zLength + offset + 0.3)])
        dimLineGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([dimLineGeometry, sideGeometry]);
    }

    for (let i = 0; i < numberInZ; i++) {//Side lines of inner z-dimensions
        sideGeometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(this.xLength + offset - 0.3, 0, coordZ[i]), new THREE.Vector3(this.xLength + offset + 0.3, 0, coordZ[i])])
        dimLineGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([dimLineGeometry, sideGeometry]);
    }

    //Left side line of toatl x-dimension
    sideGeometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(0, 0, this.zLength + 2*offset - 0.3), new THREE.Vector3(0, 0, this.zLength + 2*offset + 0.3)])
    dimLineGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([dimLineGeometry, sideGeometry]);

    //Right side line of toatl x-dimension
    sideGeometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(this.xLength, 0, this.zLength + 2*offset - 0.3), new THREE.Vector3(this.xLength, 0, this.zLength + 2*offset + 0.3)])
    dimLineGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([dimLineGeometry, sideGeometry]);

    //Left side line of toatl x-dimension
    sideGeometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(this.xLength + 2 * offset - 0.3, 0, 0), new THREE.Vector3(this.xLength + 2 * offset + 0.3,0, 0)])
    dimLineGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([dimLineGeometry, sideGeometry]);

    //Left side line of toatl x-dimension
    sideGeometry = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(this.xLength + 2 * offset - 0.3, 0, this.zLength), new THREE.Vector3(this.xLength + 2 * offset + 0.3, 0, this.zLength)])
    dimLineGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([dimLineGeometry, sideGeometry]);

    //Inner x-dimension line
    dimLineGeometry1 = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(0, 0, this.zLength + offset), new THREE.Vector3(this.xLength, 0, this.zLength + offset)]);
    dimLineGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([dimLineGeometry, dimLineGeometry1]);

    //Total x-dimension line
    dimLineGeometry1 = new THREE.BufferGeometry().setFromPoints([new THREE.Vector3(0, 0, this.zLength + 2 * offset), new THREE.Vector3(this.xLength, 0, this.zLength + 2 * offset)]);
    dimLineGeometry = THREE.BufferGeometryUtils.mergeBufferGeometries([dimLineGeometry, dimLineGeometry1]);

    this.dimensions = new THREE.LineSegments(dimLineGeometry, dimMaterial);   

    ///////////////////Dimensions text
    geoProperties.size = 0.5;
    let dimTextGeo = new THREE.TextBufferGeometry(`${coordX[1]} m`, geoProperties);
    dimTextGeo.applyMatrix4(matrix.makeRotationX(-Math.PI / 2))
        .applyMatrix4(matrix.makeTranslation(0.5 * coordX[1] - geoProperties.size, 0, this.zLength + offset - 0.5 * geoProperties.size));

    for (let i = 2; i < numberInX; i++) {
        let geo = new THREE.TextBufferGeometry(`${(10 * coordX[i] - 10 * coordX[i - 1]) / 10} m`, geoProperties);
        geo.applyMatrix4(matrix.makeRotationX(-Math.PI / 2))
            .applyMatrix4(matrix.makeTranslation(0.5 * (coordX[i] + coordX[i - 1]) - geoProperties.size, 0, this.zLength + offset - 0.5 * geoProperties.size));
        dimTextGeo = THREE.BufferGeometryUtils.mergeBufferGeometries([dimTextGeo, geo]);
    }

    let xTotalGeo = new THREE.TextBufferGeometry(`${this.xLength} m`, geoProperties);
    xTotalGeo.applyMatrix4(matrix.makeRotationX(-Math.PI / 2))
        .applyMatrix4(matrix.makeTranslation(0.5 * this.xLength - geoProperties.size, 0, this.zLength + 2 * offset - 0.5 * geoProperties.size));
    dimTextGeo = THREE.BufferGeometryUtils.mergeBufferGeometries([dimTextGeo, xTotalGeo]);

    for (let i = 1; i < numberInZ; i++) {
        let geo = new THREE.TextBufferGeometry(`${(10 * coordZ[i] - 10 * coordZ[i - 1]) / 10} m`, geoProperties);
        geo.applyMatrix4(matrix.makeRotationY(Math.PI / 2)).applyMatrix4(matrix.makeRotationZ(Math.PI / 2))
            .applyMatrix4(matrix.makeTranslation(this.xLength + offset - 0.5 * geoProperties.size, 0, 0.5 * (coordZ[i] + coordZ[i - 1]) + geoProperties.size));
        dimTextGeo = THREE.BufferGeometryUtils.mergeBufferGeometries([dimTextGeo, geo]);
    }

    let zTotalGeo = new THREE.TextBufferGeometry(`${this.zLength} m`, geoProperties);
    zTotalGeo.applyMatrix4(matrix.makeRotationY(Math.PI / 2)).applyMatrix4(matrix.makeRotationZ(Math.PI / 2))
        .applyMatrix4(matrix.makeTranslation(this.xLength + 2 * offset - 0.5 * geoProperties.size, 0, 0.5 * this.zLength + geoProperties.size));
    dimTextGeo = THREE.BufferGeometryUtils.mergeBufferGeometries([dimTextGeo, zTotalGeo]);

    this.dimensions.add(new THREE.Mesh(dimTextGeo, new THREE.MeshBasicMaterial({ color: 0x0000ff, opacity: 0.7, transparent: true })))
    //#endregion
}