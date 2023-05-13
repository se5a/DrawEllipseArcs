using System.Numerics;
using DVec;
using Pulsar4X.SDL2UI;

namespace DrawEllipse;

public class Stuff
{
    public static (int x, int y)[] EllipseArray(float semiMajorAxis, float semiMinorAxis, float startAngle, float endAngle, int numPoints)
    {
        (int x, int y)[] points = new (int x, int y)[numPoints];

        float radius = (semiMajorAxis + semiMinorAxis) / 2.0f; // average radius

        float angleRange = endAngle - startAngle;

        float prevX = radius * (float)Math.Cos(startAngle);
        float prevY = radius * (float)Math.Sin(startAngle);

        points[0] = new ((int)prevX, (int)prevY);

        for (int i = 1; i < numPoints; i++)
        {
            float t = startAngle + angleRange * (float)i / numPoints;

            float x = radius * (float)Math.Cos(t);
            float y = radius * (float)Math.Sin(t);

            float delta = angleRange / (float)(numPoints - 1);

            float alpha = (float)Math.Atan2(y - prevY, x - prevX);

            float nextX = x - delta * radius * (float)Math.Sin(alpha) / (float)Math.Sin(delta / 2);
            float nextY = y + delta * radius * (float)Math.Cos(alpha) / (float)Math.Sin(delta / 2);

            points[i] = ((int)nextX, (int)nextY);

            prevX = nextX;
            prevY = nextY;
        }

        return points;
    }


    public static DVec.Vector2[] EllipseArrayFromPaper(double semiMaj, double semiMin, double tilt, int numPoints)
    {
        DVec.Vector2[] points = new DVec.Vector2[numPoints];

        var n = numPoints;
        var a = semiMaj;
        var b = semiMin;
        var xc = 0; //this is the center position of the ellipse;
        var yc = 0;
        
        var deltaPhi = 2 * Math.PI / (n - 1);
        var ct = Math.Cos(tilt);
        var st = Math.Sin(tilt);
        var cdp = Math.Cos(deltaPhi);
        var sdp = Math.Sin(deltaPhi);

        var alpha = cdp + sdp * st * ct * (a / b - b / a);
        var bravo = - sdp * ( (b * st) * (b * st) + (a * ct) * (a * ct)) / (a * b);
        var charli = sdp * ( (b * ct) * (b * ct) + (a * st) * (a * st)) / (a * b);
        var delta = cdp + sdp * st * ct * (b / a - a / b);
        delta = delta - (charli * bravo) / alpha;
        charli = charli / alpha;

        var x = a * ct;
        var y = a * st;

        for (int i = 0; i < numPoints; i++)
        {
            var xn = xc + x;
            var yn = yc + y;
            points[i] = new DVec.Vector2(xn, yn);
            x = alpha * x + bravo * y;
            y = charli * x + delta * y;
        }
        
        return points;
    }
    
    public static (int x, int y)[] EllipseArrayFromPaper2(double semiMaj, double semiMin, double tilt, int numPoints)
    {
        (int x, int y)[] points = new (int x, int y)[numPoints];

        var n = numPoints;
        var a = semiMaj;
        var b = semiMin;
        var xc = 0; //this is the center position of the ellipse;
        var yc = 0;
        
        var deltaPhi = 2 * Math.PI / (n - 1);
        var ct = Math.Cos(tilt);
        var st = Math.Sin(tilt);
        var cdp = Math.Cos(deltaPhi);
        var sdp = Math.Sin(deltaPhi);

        var alpha = cdp + sdp * st * ct * (a / b - b / a);
        var bravo = - sdp * ( (b * st) * (b * st) + (a * ct) * (a * ct)) / (a * b);
        var charli = sdp * ( (b * ct) * (b * ct) + (a * st) * (a * st)) / (a * b);
        var delta = cdp + sdp * st * ct * (b / a - a / b);
        delta = delta - (charli * bravo) / alpha;
        charli = charli / alpha;

        var x = a * ct;
        var y = a * st;

        for (int i = 0; i < numPoints; i++)
        {
            var xn = xc + x;
            var yn = yc + y;
            points[i] = ((int)xn, (int)yn);
            x = alpha * x + bravo * y;
            y = charli * x + delta * y;
        }
        
        return points;
    }
    
    public static (int x, int y)[] EllipseArraySimple(double semiMaj, double semiMin, int numPoints)
    {
        (int x, int y)[] points = new (int x, int y)[numPoints];

        var n = numPoints;
        var a = semiMaj;
        var b = semiMin;
        var xc = 0; //this is the center position of the ellipse;
        var yc = 0;
        
        var deltaPhi = 2 * Math.PI / (n - 1);
        var cdp = Math.Cos(deltaPhi);
        var sdp = Math.Sin(deltaPhi);

        var alpha = cdp;
        var bravo = - sdp * (a * a) / (a * b);
        var charli = sdp * ( b * b) / (a * b);
        var delta = cdp;
        delta = delta - (charli * bravo) / alpha;
        charli = charli / alpha;

        double x = a;
        double y = 0;

        for (int i = 0; i < numPoints; i++)
        {
            var xn = xc + x;
            var yn = yc + y;
            points[i] = ((int)xn, (int)yn);
            x = alpha * x + bravo * y;
            y = charli * x + delta * y;
        }
        
        return points;
    }
    
    public static DVec.Vector2[] EllipseArrayMTX(double semiMaj, double eccentricity, double tilt, int numPoints)
    {
        DVec.Vector2[] points = new DVec.Vector2[numPoints];
        
        var n = numPoints;
        var a = semiMaj;
        var b = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        var xc = 0; //this is the center position of the ellipse;
        var yc = 0;
        var linEcc = Math.Sqrt(a * a - b * b);
        
        var deltaPhi = 2 * Math.PI / (n - 1);
        var cdp = Math.Cos(deltaPhi);
        var sdp = Math.Sin(deltaPhi);

        var alpha = cdp;
        var bravo = - sdp * (a * a) / (a * b);
        var charli = sdp * ( b * b) / (a * b);
        var delta = cdp - (charli * bravo) / alpha;
        charli = charli / alpha;

        double x = a;
        double y = 0;

        for (int i = 0; i < numPoints; i++)
        {
            var xn = xc + x;
            var yn = yc + y;
            points[i] = new DVec.Vector2(xn, yn);
            x = alpha * x + bravo * y;
            y = charli * x + delta * y;
        }

        var moveMtx = Matrix.IDTranslate(linEcc, 0);
        var rotMtx = Matrix.IDRotate(tilt);
        var endMtx = moveMtx * rotMtx;
        var pnts = endMtx.Transform(points);
        
        return pnts;
    }
    
    public static DVec.Vector2[] EllipsArrayPaper1(double semiMaj, double eccentricity, double tilt, double start, double end, int numPoints)
    {
        DVec.Vector2[] points = new DVec.Vector2[numPoints];
        double deltaPhi = 2 * Math.PI / (numPoints - 1);
        double ct = Math.Cos(tilt);
        double st = Math.Sin(tilt);
        double cdp = Math.Cos(deltaPhi);
        double sdp = Math.Sin(deltaPhi);
        double cndp = 1.0;
        double sndp = 0;
        double a = semiMaj;
        double b = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        double xc = 0;
        double yc = 0;
        
        for (int i = 0; i < numPoints; i++)
        {
            double x = a * cndp;
            double y = b * sndp;

            double xi = xc + x * ct - y * st;
            double yi = yc + x * st + y * ct;
            points[i] = new DVec.Vector2(xi, yi);

            double temp = cndp * cdp - sndp * sdp;
            sndp = sndp * cdp + cndp * sdp;
            cndp = temp;
        }

        return points;
    }

    public static DVec.Vector2[] EllipseFocalPolar(double semiMaj, double eccentricity, double tilt, double start,
        double end, int numPoints)
    {
        DVec.Vector2[] points = new DVec.Vector2[numPoints*2];
        
        var a = semiMaj;
        var e = eccentricity;
        var b = semiMaj * Math.Sqrt(1 - e * e);
        var linEcc = Math.Sqrt(a * a - b * b);
        
        var sweep = end - start;
        double Δθ = sweep / (numPoints - 1);
        double ct = Math.Cos(tilt);
        double st = Math.Sin(tilt);

        
        for (int i = 0; i < numPoints; i++)
        {
            // get azimuth angle along the ellipse
            double θ = start + i * Δθ;
            // get polar coordinate of ellipse r(θ)
            double r = a * (1 - e * e) / (1 + e * Math.Cos(θ));
            
            // get axis aligned (x,y) point from foci
            double x = r * Math.Cos(θ); 
            double y = r * Math.Sin(θ);            

            // get rotated (x,y) point
            var xx = ct * x - st * y;
            var yy = st * x + ct * y;
            DVec.Vector2 localPoint = new DVec.Vector2(xx, yy);
            points[i] = localPoint;
            
            double θ2 = -start + i * Δθ;
            double x2 = -x - linEcc * 2;//x2 = -r * Math.Cos(θ);
            double y2 = -y; //-r * Math.Sin(θ);
            var xx2 = ct * x2 - st * y2;
            var yy2 = st * x2 + ct * y2;
            localPoint = new DVec.Vector2(xx2, yy2);
            points[i+numPoints] = localPoint;
        }

        return points;
    }

    public static DVec.Vector2[] EllipseFullMtx(double semiMaj, double eccentricity, double tilt, double start,
        double end, int numPoints)
    {
        double b = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        start = Math.Atan(semiMaj / b * Math.Tan(start));
        end = Math.Atan(semiMaj / b * Math.Tan(end));
        
        double Δθ = 2 * Math.PI / (numPoints - 1);
        var sweep = end - start;
        numPoints = (int)Math.Abs(sweep / Δθ);
        DVec.Vector2[] points = new DVec.Vector2[numPoints + 1];
        double θ = 0;
        double x = 0;
        double y = 0;
        
        double linEcc = Math.Sqrt(semiMaj * semiMaj - b * b);
        
        for (int i = 0; i < numPoints; i++)
        {
            θ = start + Δθ * i;
            x = semiMaj * Math.Cos(θ);
            y = semiMaj * Math.Sin(θ);

            points[i] = new DVec.Vector2(x, y);
        }
        x = semiMaj * Math.Cos(end);
        y = semiMaj * Math.Sin(end);
        points[numPoints] = new DVec.Vector2(x, y);

        var scalemtx = Matrix.IDScale(1, eccentricity);
        var moveMtx = Matrix.IDTranslate(linEcc, 0);
        var rotMtx = Matrix.IDRotate(tilt);
        var endMtx = scalemtx * moveMtx * rotMtx;
        points = endMtx.Transform(points);
        
        return points;
    }
    
    public static DVec.Vector2[] EllipseFullMtxSweep(double semiMaj, double eccentricity, double tilt, double start,
        double sweep, int numPoints)
    {
        
        //convert ellipse angles to circle angles. 
        double b = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        //start = Math.Atan2(b * Math.Cos(start), semiMaj * Math.Sin(start));
        //sweep = Math.Atan2(b * Math.Cos(sweep), semiMaj * Math.Sin(sweep));
        double end = start + sweep;
        double Δθ = 2 * Math.PI / (numPoints - 1) * Math.Sign(sweep); //arc increment for a whole circle
        numPoints = (int)Math.Abs(sweep / Δθ); //numpoints for just the arc
        DVec.Vector2[] points = new DVec.Vector2[numPoints + 1];
        
        double linEcc = Math.Sqrt(semiMaj * semiMaj - b * b);
        double θ = 0;
        double x = 0;
        double y = 0;
        
        for (int i = 0; i < numPoints; i++)
        {
            θ = start + Δθ * i;
            x = semiMaj * Math.Cos(θ);
            y = semiMaj * Math.Sin(θ);
            points[i] = new DVec.Vector2(x, y);
        }
        x = semiMaj * Math.Cos(end);
        y = semiMaj * Math.Sin(end);
        points[numPoints] = new DVec.Vector2(x, y);

        Matrix mirMtx = Matrix.IDMirror(true, false);
        Matrix scalemtx = Matrix.IDScale(1,  1 - eccentricity);
        Matrix moveMtx = Matrix.IDTranslate(-linEcc, 0);
        Matrix rotMtx = Matrix.IDRotate(tilt);
        Matrix endMtx = moveMtx * scalemtx * rotMtx;
        points = endMtx.Transform(points);
        
        return points;
    }

    public static DVec.Vector2[] CheatsCircle(double semiMaj, double eccentricity, double tilt, double start,
        double sweep, int numPoints)
    {
        
        //convert ellipse angles to circle angles. 
        double b = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        //start = Math.Atan2(b * Math.Cos(start), semiMaj * Math.Sin(start));
        //sweep = Math.Atan2(b * Math.Cos(sweep), semiMaj * Math.Sin(sweep));
        double end = start + sweep;
        double Δθ = 2 * Math.PI / (numPoints - 1) * Math.Sign(sweep); //arc increment for a whole circle
        numPoints = (int)Math.Abs(sweep / Δθ); //numpoints for just the arc
        DVec.Vector2[] points = new DVec.Vector2[numPoints + 1];
        
        double linEcc = Math.Sqrt(semiMaj * semiMaj - b * b);
        double θ = 0;
        double x = semiMaj;
        double y = 0;
        
        for (int i = 0; i < numPoints; i++)
        {
            x -= Δθ * y;
            y += Δθ * x;
            points[i] = new DVec.Vector2(x, y);
        }
        x = semiMaj * Math.Cos(end);
        y = semiMaj * Math.Sin(end);
        points[numPoints] = new DVec.Vector2(x, y);

        Matrix mirMtx = Matrix.IDMirror(true, false);
        Matrix scalemtx = Matrix.IDScale(1,  1 - eccentricity);
        Matrix moveMtx = Matrix.IDTranslate(-linEcc, 0);
        Matrix rotMtx = Matrix.IDRotate(tilt);
        Matrix endMtx = moveMtx * scalemtx * rotMtx;
        points = endMtx.Transform(points);
        
        return points;
    }

    
}