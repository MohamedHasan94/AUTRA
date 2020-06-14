let loader = new THREE.FontLoader();
let myFont;

loader.load('../lib/three.js/helvetiker_regular.typeface.json', function (font) {
    myFont = font;
    loader = null;
});


let loadMaterial = new THREE.MeshBasicMaterial({ color: 0xcc0000, transparent: true, opacity: 0.3, side: THREE.DoubleSide });
let fontMaterial = new THREE.MeshBasicMaterial({ color: 0x000000 });

class Load {
    constructor(magnitude, pattern) {
        this.magnitude = magnitude * -1;
        this.pattern = pattern;
    }
    clone() {
        return new this.constructor(-this.magnitude, this.pattern);
    }
}

class LineLoad extends Load {
    constructor(magnitude, pattern) {
        super(magnitude, pattern);
    }
    render(beam) {
        if (this.magnitude) {
            let height = (-0.42 * this.magnitude + 0.395);
            height = height < 0.5 ? 0.5 : height > 2.5 ? 2.5 : height;

            let mesh = new THREE.Mesh(new THREE.PlaneBufferGeometry(beam.data.length, height, 1, 1), loadMaterial.clone());

            let magnitude = -1 * this.magnitude;
            let textGeometry = new THREE.TextBufferGeometry(`${-1 * this.magnitude} t/m`, {
                font: myFont,
                size: 0.35 * height,
                height: 0,
                curveSegments: 3,
                bevelEnabled: false
            });
            let text = new THREE.Mesh(textGeometry, fontMaterial);
            text.position.x -= 0.5 * height;
            mesh.add(text);

            mesh.position.copy(beam.visual.mesh.position);
            mesh.position.add(beam.visual.direction.clone().multiplyScalar(0.5 * beam.data.length));
            mesh.position.y += 0.5 * height;
            mesh.rotateY(Math.PI / 2 - beam.visual.mesh.rotation.y);
            return mesh;
        } else
            return null;
    }
}

let direction = new THREE.Vector3(0, -1, 0);
class PointLoad extends Load {
    constructor(magnitude, pattern) {
        super(magnitude, pattern);
    }
    render(position) {
        let height = (-0.42 * this.magnitude + 0.395);
        height = height < 0.5 ? 0.5 : height > 2.5 ? 2.5 : height;

        position.y += height + 0.075;

        let arrow = new THREE.ArrowHelper(direction, position, height, 0xcc00ff);
        let textGeometry = new THREE.TextBufferGeometry(`${this.magnitude * -1} t`, {
            font: myFont,
            size: 0.35 * height,
            height: 0,
            curveSegments: 3,
            bevelEnabled: false
        });
        let text = new THREE.Mesh(textGeometry, fontMaterial);
        text.rotateX(Math.PI);
        arrow.add(text);
        return arrow;
    }
}
