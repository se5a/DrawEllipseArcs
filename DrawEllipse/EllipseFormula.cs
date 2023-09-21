using System.Numerics;
using DVec;
using Pulsar4X.SDL2UI;
using Vector2 = DVec.Vector2;

namespace DrawEllipse;

public class EllipseFormula
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


    /// <summary>
    /// This is a C# translation of the formula found in this paper: https://academic.oup.com/comjnl/article/14/1/81/356378
    /// </summary>
    /// <param name="semiMaj"></param>
    /// <param name="semiMin"></param>
    /// <param name="tilt"></param>
    /// <param name="numPoints"></param>
    /// <returns></returns>
    public static Vector2[] EllipseArrayFromPaper(double semiMaj, double semiMin, double tilt, int numPoints)
    {
        Vector2[] points = new Vector2[numPoints];

        var n = numPoints;
        var a = semiMaj;
        var b = semiMin;
        var c = Math.Sqrt((a * a) - (b * b));
        var xc = -c * Math.Cos(tilt); //this is the center position of the ellipse;
        var yc = -c * Math.Sin(tilt);
        
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
    
    /// <summary>
    /// this was a translation of the paper but removing tilt,
    /// as an attempt to get a better understanding of what the paper was doing. 
    /// </summary>
    /// <param name="semiMaj"></param>
    /// <param name="semiMin"></param>
    /// <param name="numPoints"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// The paper has an un-optomised version of the function. this is it. 
    /// </summary>
    /// <param name="semiMaj"></param>
    /// <param name="eccentricity"></param>
    /// <param name="tilt"></param>
    /// <param name="numPoints"></param>
    /// <returns></returns>
    public static DVec.Vector2[] EllipsArrayPaperUnOptomised(double semiMaj, double eccentricity, double tilt, int numPoints)
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
    
    /// <summary>
    /// paper version but uses matrix to only move so we're centered around the focal point. 
    /// </summary>
    /// <param name="semiMaj"></param>
    /// <param name="eccentricity"></param>
    /// <param name="tilt"></param>
    /// <param name="numPoints"></param>
    /// <returns></returns>
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
    


    /// <summary>
    /// this was an unfinished attempt at trying to get point positions simular to the paper by doing the angles
    /// from the focal points, and switching focal points half way through. 
    /// </summary>
    /// <param name="semiMaj"></param>
    /// <param name="eccentricity"></param>
    /// <param name="tilt"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="numPoints"></param>
    /// <returns></returns>
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

    
    
    
    /// <summary>
    /// Uses Matrix transforms on a circle to change the eccentricity to an ellipse. 
    /// </summary>
    /// <param name="semiMaj"></param>
    /// <param name="eccentricity"></param>
    /// <param name="tilt"></param>
    /// <param name="start"></param>
    /// <param name="sweep"></param>
    /// <param name="numPoints"></param>
    /// <returns></returns>
    public static Vector2[] EllipseFullMtxSweep(double semiMaj, double eccentricity, double tilt, double start,
        double sweep, int numPoints)
    {
        
        //convert ellipse angles to circle angles. 
        double b = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        //start = Math.Atan2(b * Math.Cos(start), semiMaj * Math.Sin(start));
        //sweep = Math.Atan2(b * Math.Cos(sweep), semiMaj * Math.Sin(sweep));
        
        double end = start + sweep;
        double Δθ = 2 * Math.PI / (numPoints - 1) * Math.Sign(sweep); //arc increment for a whole circle
        numPoints = (int)Math.Abs(sweep / Δθ); //numpoints for just the arc
        Vector2[] points = new Vector2[numPoints + 1];
        
        double linEcc = Math.Sqrt(semiMaj * semiMaj - b * b);
        double θ = 0;
        double x = 0;
        double y = 0;
        
        for (int i = 0; i < numPoints; i++)
        {
            θ = start + Δθ * i;
            x = semiMaj * Math.Cos(θ);
            y = semiMaj * Math.Sin(θ);
            points[i] = new Vector2(x, y);
        }
        x = semiMaj * Math.Cos(end);
        y = semiMaj * Math.Sin(end);
        points[numPoints] = new Vector2(x, y);

        Matrix mirMtx = Matrix.IDMirror(true, false);
        Matrix scalemtx = Matrix.IDScale(1,  b/semiMaj);
        Matrix moveMtx = Matrix.IDTranslate(-linEcc, 0);
        Matrix rotMtx = Matrix.IDRotate(-tilt);
        Matrix endMtx = moveMtx * scalemtx * rotMtx;
        points = endMtx.Transform(points);
        
        return points;
    }


    /// <summary>
    /// Uses a cheat way to position points for the circle, faster than using trig but not accurate? 
    /// </summary>
    /// <param name="semiMaj"></param>
    /// <param name="eccentricity"></param>
    /// <param name="tilt"></param>
    /// <param name="start"></param>
    /// <param name="sweep"></param>
    /// <param name="numPoints"></param>
    /// <returns></returns>
    public static Vector2[] CheatsCircle(double semiMaj, double eccentricity, double tilt, double start,
        double sweep, int numPoints)
    {
        
        //convert ellipse angles to circle angles. 
        double b = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        //start = Math.Atan2(b * Math.Cos(start), semiMaj * Math.Sin(start));
        //sweep = Math.Atan2(b * Math.Cos(sweep), semiMaj * Math.Sin(sweep));
        double end = start + sweep;
        double Δθ = 2 * Math.PI / (numPoints - 1) * Math.Sign(sweep); //arc increment for a whole circle
        numPoints = (int)Math.Abs(sweep / Δθ); //numpoints for just the arc
        Vector2[] points = new Vector2[numPoints + 1];
        
        double linEcc = Math.Sqrt(semiMaj * semiMaj - b * b);
        double θ = 0;
        double x = semiMaj;
        double y = 0;
        
        for (int i = 0; i < numPoints; i++)
        {
            x -= Δθ * y;
            y += Δθ * x;
            points[i] = new Vector2(x, y);
        }
        x = semiMaj * Math.Cos(end);
        y = semiMaj * Math.Sin(end);
        points[numPoints] = new Vector2(x, y);
        
        Matrix scalemtx = Matrix.IDScale(1,  1 - eccentricity);
        Matrix moveMtx = Matrix.IDTranslate(-linEcc, 0);
        Matrix rotMtx = Matrix.IDRotate(tilt);
        Matrix endMtx = moveMtx * scalemtx * rotMtx;
        points = endMtx.Transform(points);
        
        return points;
    }
    
    public static DVec.Vector2[] ArcWithFocalAngle(double semiMaj, double eccentricity, double tilt, double start,
        double sweep, int numPoints)
    {
        double θ = 0;
        double x = 0;
        double y = 0;
        
        double b = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        double linEcc = Math.Sqrt(semiMaj * semiMaj - b * b);
        
        //convert focal angle to center angle
        x = linEcc * Math.Cos(start);
        y = linEcc * Math.Sin(start);
        //shift y from ellipse to circle:
        y = y * -(1 - eccentricity);
        start = Math.Atan2(y, x);
        
        //convert focal angle to center angle
        x = linEcc * Math.Cos(sweep);
        y = linEcc * Math.Sin(sweep);
        //shift y from ellipse to circle:
        y = y * -(1 - eccentricity);
        sweep = Math.Atan2(y, x);
        
        
        
        double end = start + sweep;
        
        double Δθ = 2 * Math.PI / (numPoints - 1) * Math.Sign(sweep); //arc increment for a whole circle
        numPoints = (int)Math.Abs(sweep / Δθ); //numpoints for just the arc
        DVec.Vector2[] points = new DVec.Vector2[numPoints + 1];
        
        

        
        for (int i = 0; i < numPoints; i++)
        {
            θ = -start - Δθ * i;
            x = semiMaj * Math.Cos(θ);
            y = semiMaj * Math.Sin(θ);
            points[i] = new DVec.Vector2(x, y);
        }
        x = semiMaj * Math.Cos(-end);
        y = semiMaj * Math.Sin(-end);
        points[numPoints] = new DVec.Vector2(x, y);
        
        Matrix scalemtx = Matrix.IDScale(1,  1 - eccentricity);
        Matrix moveMtx = Matrix.IDTranslate(-linEcc, 0);
        Matrix rotMtx = Matrix.IDRotate(tilt);
        Matrix endMtx = moveMtx * scalemtx * rotMtx;
        points = endMtx.Transform(points);
        
        return points;
    }
    
    public static Vector2[] ArcWithFocalAngle(double semiMaj, double eccentricity, double tilt, Vector2 start,
        Vector2 end, int numPoints)
    {
        double θ = 0;
        double x = 0;
        double y = 0;
        //convert ellipse angles to circle angles. 
        double b = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        double linEcc = Math.Sqrt(semiMaj * semiMaj - b * b);
        
        //convert focal angle to center angle
        x = start.X + linEcc;
        y = start.Y * -(1 - eccentricity);//shift y from ellipse to circle

        double startAng = Math.Atan2(y, x);
        
        //convert focal angle to center angle
        x = end.X + linEcc;
        y = end.Y * -(1 - eccentricity);//shift y from ellipse to circle

        double endAng = Math.Atan2(y, x);
        double sweep = endAng - startAng;
        
        double Δθ = 2 * Math.PI / (numPoints - 1) * Math.Sign(sweep); //arc increment for a whole circle
        numPoints = (int)Math.Abs(sweep / Δθ); //numpoints for just the arc
        Vector2[] points = new Vector2[numPoints + 1];
        
        for (int i = 0; i < numPoints; i++)
        {
            θ = -startAng - Δθ * i;
            x = semiMaj * Math.Cos(θ);
            y = semiMaj * Math.Sin(θ);
            points[i] = new DVec.Vector2(x, y);
        }
        x = semiMaj * Math.Cos(-endAng);
        y = semiMaj * Math.Sin(-endAng);
        points[numPoints] = new DVec.Vector2(x, y);
        
        Matrix scalemtx = Matrix.IDScale(1,  1 - eccentricity);
        Matrix moveMtx = Matrix.IDTranslate(-linEcc, 0);
        Matrix rotMtx = Matrix.IDRotate(tilt);
        Matrix endMtx = moveMtx * scalemtx * rotMtx;
        points = endMtx.Transform(points);
        
        return points;
    }


    /// <summary>
    /// Positions done using radius from focal. 
    /// </summary>
    /// <param name="semiMaj"></param>
    /// <param name="eccentricity"></param>
    /// <param name="tilt"></param>
    /// <param name="start"></param>
    /// <param name="sweep"></param>
    /// <param name="numPoints"></param>
    /// <returns></returns>
    public static Vector2[] ArcRadiusFromFocal(double semiMaj, double eccentricity, double tilt, double start, double sweep,
        int numPoints)
    {                    
        var _semiMinor = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        double linEcc = Math.Sqrt(semiMaj * semiMaj - _semiMinor * _semiMinor);
        
        double θ = 0;
        double x = 0;
        double y = 0;
        double r = RadiusFromFocal(semiMaj, eccentricity, tilt, start);
        double Δθ = 2 * Math.PI / (numPoints - 1) * Math.Sign(sweep); 
        numPoints = (int)Math.Abs(sweep / Δθ) + 1; //numpoints for just the arc
        
        Vector2[] points = new Vector2[numPoints + 1];
        for (int i = 0; i < numPoints; i++)
        {
            θ = start + Δθ * i;
            r = RadiusFromFocal(semiMaj, eccentricity, tilt, θ);
            x = r * Math.Cos(θ);
            y = r * Math.Sin(θ);
            points[i] = new Vector2(x, y);
        }
        //lastPoint:
        θ = start + sweep;
        r = RadiusFromFocal(semiMaj, eccentricity, tilt, θ);
        points[^1] = new Vector2()
        {
            X = r * Math.Cos(θ),
            Y = r * Math.Sin(θ)
        };

        return points;
    }
    
    /// <summary>
    /// Positions done using radius from focal. 
    /// </summary>
    /// <param name="semiMaj"></param>
    /// <param name="eccentricity"></param>
    /// <param name="tilt"></param>
    /// <param name="start"></param>
    /// <param name="sweep"></param>
    /// <param name="numPoints"></param>
    /// <returns></returns>
    public static Vector2[] ArcRadiusFromFocal(double semiMaj, double eccentricity, double tilt, Vector2 startPnt, Vector2 endPnt,
        int numPoints)
    {                    
        var _semiMinor = semiMaj * Math.Sqrt(1 - eccentricity * eccentricity);
        double linEcc = Math.Sqrt(semiMaj * semiMaj - _semiMinor * _semiMinor);
        double startAng = Math.Atan2(startPnt.Y, startPnt.X);
        double endAng =  Math.Atan2(endPnt.Y, endPnt.X);
        double sweep = Angle.NormaliseRadiansPositive( endAng - startAng);
        
        double θ = 0;
        double x = 0;
        double y = 0;
        double r = RadiusFromFocal(semiMaj, eccentricity, tilt, startAng);
        double Δθ = 2 * Math.PI / (numPoints - 1) * Math.Sign(sweep); 
        if (Δθ == 0)
        {
            return new Vector2[]
            {
                startPnt,
                endPnt
            };
        }
        numPoints = (int)Math.Abs(sweep / Δθ) + 1; //numpoints for just the arc
        
        Vector2[] points = new Vector2[numPoints + 1];
        for (int i = 0; i < numPoints; i++)
        {
            θ = startAng + Δθ * i;
            r = RadiusFromFocal(semiMaj, eccentricity, tilt, θ);
            x = r * Math.Cos(θ);
            y = r * Math.Sin(θ);
            points[i] = new Vector2(x, y);
        }
        //lastPoint:
        θ = endAng;
        r = RadiusFromFocal(semiMaj, eccentricity, tilt, θ);
        points[^1] = endPnt;

        return points;
    }
    
    /// <summary>
    /// https://en.wikipedia.org/wiki/Ellipse#Polar_form_relative_to_focus
    /// This is a True Anomaly
    /// </summary>
    /// <param name="a">semi major</param>
    /// <param name="e">eccentricy</param>
    /// <param name="phi">tilt, angle from focal 1 to focal 2 (or center)</param>
    /// <param name="theta">angle</param>
    /// <returns></returns>
    public static double RadiusFromFocal(double a, double e, double phi, double theta)
    {
        double dividend = a * (1 - e * e);
        double divisor = 1 + e * Math.Cos(theta - phi);
        double quotent = dividend / divisor;
        return quotent;
    }


    public static double RadiusFromCenter(double b, double e,double phi, double theta)
    {
        return b / Math.Sqrt(1 - (e * Math.Cos(theta - phi)));
    }
}